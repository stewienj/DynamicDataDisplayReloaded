﻿using DynamicDataDisplay.Common.Auxiliary;
using DynamicDataDisplay.Maps;
using DynamicDataDisplay.Maps.Properties;
using DynamicDataDisplay.Maps.Servers.Network;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace DynamicDataDisplay.Charts.Maps
{
	public abstract class NetworkTileServer : SourceTileServer, IWeakEventListener
	{
		protected NetworkTileServer()
		{
			NetworkAvailabilityManager.AddListener(this);

			isNetworkAvailable = NetworkInterface.GetIsNetworkAvailable();
		}

		private bool isNetworkAvailable = false;

		protected sealed override string GetCustomName()
		{
			return "Network " + ServerName;
		}

		#region ITileServer Members

		public override bool Contains(TileIndex id)
		{
			return isNetworkAvailable && MinLevel <= id.Level && id.Level <= MaxLevel;
		}

		private int runningDownloadsNum = 0;
		protected int RunningDownloadsNum
		{
			get { return runningDownloadsNum; }
		}

		private readonly ConcurrentStack<TileIndex> waitingIDs = new ConcurrentStack<TileIndex>();

		private bool firstCall = true;
		public override void BeginLoadImage(TileIndex id)
		{
			VerifyTileIndex(id);

			string uri = CreateRequestUriCore(MapTileProvider.Normalize(id));

			bool useMultipleServers = ServersNum != 0;
			if (useMultipleServers)
			{
				CurrentServer++;
				if (CurrentServer >= MinServer + ServersNum)
				{
					CurrentServer = MinServer;
				}
			}

			if (runningDownloadsNum >= MaxConcurrentDownloads)
			{
				waitingIDs.Push(id);
				return;
			}

			MapsTraceSource.Instance.ServerInformationTraceSource.TraceInformation("\"{0}\" - began to load url=\"{1}\"", GetCustomName(), uri);

			WebRequest request = WebRequest.Create(uri);
			request.Proxy.Credentials = CredentialCache.DefaultCredentials;
			AdjustRequest(request);

			runningDownloadsNum++;

			// this is hack to prevent freezing when request.BeginGetResponse was called
			// at the 1st time
			if (!firstCall)
			{
				request.BeginGetResponse(ResponseReadyCallback,
					new ResponseCallbackInfo { ID = id, Request = request });
			}
			else
			{
				Task.Factory.StartNew(() =>
				{
					request.BeginGetResponse(ResponseReadyCallback, new ResponseCallbackInfo { ID = id, Request = request });
				}).WithExceptionThrowingInDispatcher(Dispatcher);
			}
		}

		protected override void ReportFailure(TileIndex id)
		{
			runningDownloadsNum--;

			BeginLoadImageFromQueue();

			base.ReportFailure(id);
		}

		private void BeginLoadImageFromQueue()
		{
			if (runningDownloadsNum < MaxConcurrentDownloads)
			{
				TileIndex id = new TileIndex();
				if (waitingIDs.TryPop(out id))
				{
					//Debug.WriteLine("ID = " + id + " removed from queue");
					BeginLoadImage(id);
				}
			}
		}

		protected override void ReportSuccess(Stream stream, BitmapSource bmp, TileIndex id)
		{
			runningDownloadsNum--;

			BeginLoadImageFromQueue();

			base.ReportSuccess(stream, bmp, id);
		}

		protected virtual void AdjustRequest(WebRequest request)
		{
			HttpWebRequest r = (HttpWebRequest)request;
			r.UserAgent = UserAgent;
			if (!string.IsNullOrEmpty(Referer))
			{
				r.Referer = Referer;
			}
		}


		private void VerifyTileIndex(TileIndex id)
		{
			if (id.Level < MinLevel || id.Level > MaxLevel)
				throw new ArgumentException(
					string.Format(
					Resources.InvalidTileLevel,
						id.Level, MinLevel, MaxLevel),
					"id");
		}

		private void ResponseReadyCallback(IAsyncResult ar)
		{
			ResponseCallbackInfo info = (ResponseCallbackInfo)ar.AsyncState;
			firstCall = false;
			try
			{
				var response = info.Request.EndGetResponse(ar);

				bool goodTile = IsGoodTileResponse(response);
				if (goodTile)
				{
					UpdateStatistics(() =>
					{
						Statistics.LongValues["DownloadedBytes"] += response.ContentLength;
						Statistics.IntValues["ImagesLoaded"]++;
					});

					BeginLoadBitmapImpl(response.GetResponseStream(), info.ID);
				}
				else
				{
					ReportFailure(info.ID);
				}
			}
			catch (WebException exc)
			{
				string responseUri = exc.Response != null ? exc.Response.ResponseUri.ToString() : "Response=null";

				MapsTraceSource.Instance.ServerInformationTraceSource.TraceInformation("{0} Network \"{1}\" Failure: url=\"{2}\": {3}", DateTime.Now, ServerName, responseUri, exc.Message);

				ReportFailure(info.ID);
			}
		}

		protected virtual bool IsGoodTileResponse(WebResponse response)
		{
			return true;
		}

		public string CreateRequestUri(TileIndex id)
		{
			return CreateRequestUriCore(id);
		}

		protected abstract string CreateRequestUriCore(TileIndex index);

		#endregion

		private sealed class ResponseCallbackInfo
		{
			public WebRequest Request { get; set; }
			public TileIndex ID { get; set; }
		}

		~NetworkTileServer()
		{
			NetworkAvailabilityManager.RemoveListener(this);
		}

		#region IWeakEventListener Members

		public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
		{
			if (managerType != typeof(NetworkAvailabilityManager))
				return false;

			Dispatcher.BeginInvoke((Action)(() =>
			{
				isNetworkAvailable = NetworkInterface.GetIsNetworkAvailable();
				RaiseChangedEvent();
			}));

			return true;
		}

		#endregion
	}
}
