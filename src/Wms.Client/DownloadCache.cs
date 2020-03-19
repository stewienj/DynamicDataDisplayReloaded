// $File: //depot/WMS/WMS Overview/Wms.Client/DownloadCache.cs $ $Revision: #1 $ $Change: 20 $ $DateTime: 2004/05/23 23:42:06 $

namespace Wms.Client
{
	///
	/// Provides a file-based caching mechanism for retrieved WMS info.
	/// The files are cached in the user's TEMP directory. For each file
	/// cached, a companion file with the same prefix but a suffix of
	/// .wmsuri is created. This file contains the WMS URI that was used
	/// to retrieve the cached file, which is typically an image. Prior
	/// to requesting a file from a WMS server, the application invokes
	/// the Contains method of this class. If the return value is true,
	/// the application can then call GetFileName() to retrieve the file
	/// name of the cached file, thus saving an invocation of the WMS
	/// server. The WMS URI is called a "key" in the code below.
	/// 
	/// To use the cache successfully, always invoke the CreateFilePath()
	/// function to create full-path file names for the files to cache.
	/// For example, to cache a GIF file, call CreateFilePath() to get
	/// a full path to a file, add a .gif suffix to that name, and the
	/// invoke the Add function with that file name as the second
	/// parameter, the WMS URI as the first parameter, and a boolean
	/// value of true if you indeed want the file to become part of the
	/// cache.
	/// 
	/// The cache can be primed by invoking its LoadPersisted function,
	/// which searched the cache directory and assembles a table of any
	/// existing cached URIs with corresponding cached files. This then
	/// provides persistence across invocations of WMS applications.
	/// If LoadPersisted is not called, then only the files persisted
	/// during the application's current run are available as cached
	/// files for the current run of the application.
	/// 
	/// See Wms.Client.WMSDialog.cs for an example of using the cache.
	/// 
	/// The cache is fail-safe in that it will respond correctly and
	/// silently to a corrupted cache. That is, if the URI file exists
	/// but the cached file does not, the cache will notice and not
	/// treat that file as cached.
	/// 
	/// Since the files are cached in the user's TEMP directory, Windows
	/// should delete those files during scheduled or manual disk cleanup.
	/// 
	internal class DownloadCache
	{
		System.Collections.Hashtable fileList;

		public DownloadCache()
		{
			this.fileList = new System.Collections.Hashtable();
		}

		~DownloadCache()
		{
			this.DeleteNonPersisted();
		}

		public bool Contains(string key)
		{
			// Return true if the key -- a WMS URI -- is in the cache.
			if (key == null || key.Equals(string.Empty))
				return false;
			else
				return this.fileList.Contains(key);
		}

		// Use the following to determine whether the file associated
		// with a URI is in the cache. The key is the URI that was used
		// when the file was cached, if it was cached.
		public string GetFileName(string key)
		{
			if (key == null || key.Equals(string.Empty))
				return null;
			else
				return this.fileList[key] as string;
		}

		// Returns the path to the cache directory.
		private string cacheDir
		{
			get {return System.IO.Path.GetTempPath();}
		}

		// Use the following function to generate a unique file name
		// whose path name will locate it in the cache directory.
		public string CreateFilePath()
		{
			try
			{
				string name = "WMS" + System.Guid.NewGuid().ToString("N");
				return this.cacheDir + @"\" + name;
			}
			catch (System.Exception)
			{
				return null;
			}
		}

		// Use this function for the same purpose as above, but if you
		// want the cache directory to be other than the default.
		public static string CreateFilePath(string cacheDir)
		{
			try
			{
				string name = "WMS" + System.Guid.NewGuid().ToString("N");
				return cacheDir != null ? cacheDir + @"\" + name : name;
			}
			catch (System.Exception)
			{
				return null;
			}
		}

		// This function adds files to the cache. The third argument
		// indicates whether to add the file to the persistent cache:
		// If it's true, then the file will be cached to disk; otherwise
		// the file will be cached until for the duration of this run
		// of the program.
		public void Add(string key, string path, bool persist)
		{
			if (key == null || key.Equals(string.Empty))
				return;

			string oldPath = this.fileList[key] as string;
			if (oldPath != null)
			{
				this.deleteFile(oldPath);
			}
			this.fileList[key] = path;

			if (persist)
			{
				this.persist(key, path);
			}
		}

		private void persist(string key, string name)
		{
			try
			{
				string uriPath = System.IO.Path.ChangeExtension(name, ".wmsuri");
				using (System.IO.StreamWriter sw = System.IO.File.CreateText(uriPath))
				{
					sw.Write(key);
				}
			}
			catch (System.Exception)
			{
				// ignore any problems
			}
		}

		// Searches the default cache directory to find any valid cached files.
		// If a .wmsuri file is found, and a companion file to that exists --
		// a companion is one with the same base but a different suffix -- then
		// the URI and the cached file are added to the run-time cache.
		public void LoadPersisted()
		{
			System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(this.cacheDir);
			System.IO.FileInfo[] uriFiles = dirInfo.GetFiles("*.wmsuri");
			foreach (System.IO.FileInfo uriFile in uriFiles)
			{
				string template = System.IO.Path.GetFileNameWithoutExtension(uriFile.FullName);
				System.IO.FileInfo[] cacheFiles = dirInfo.GetFiles(template + ".*");
				foreach (System.IO.FileInfo cacheFile in cacheFiles)
				{
					if (!cacheFile.Extension.Equals(".wmsuri"))
					{
						try
						{
							using (System.IO.StreamReader sr = uriFile.OpenText())
							{
								string key = sr.ReadToEnd();
								this.fileList.Add(key, cacheFile.FullName);
							}
						}
						catch (System.Exception)
						{
							// ignore any problems
						}
						break;
					}
				}
			}
		}

		// Removes all entries from the run-time cache.
		public void Clear()
		{
			foreach (string file in this.fileList.Values)
			{
				this.deleteFile(file);
			}
		}

		// Deletes files from the persistent cache.
		private void deleteFile(string path)
		{
			if (path == null || path.Equals(string.Empty))
				return;

			System.IO.FileInfo fi = new System.IO.FileInfo(path);
			if (fi.Exists)
			{
				try
				{
					fi.Delete();
				}
				catch (System.Exception e)
				{
					string msg = e.Message;
					// ignore any problems deleting file
				}
			}
		}

		// Deletes all persisted files, effectively clearing the persistent cache.
		public void DeletePersisted(System.DateTime expiryTime)
		{
			System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(this.cacheDir);
			System.IO.FileInfo[] uriFiles = dirInfo.GetFiles("*.wmsuri");
			foreach (System.IO.FileInfo uriFile in uriFiles)
			{
				string template = System.IO.Path.GetFileNameWithoutExtension(uriFile.FullName);
				System.IO.FileInfo[] cacheFiles = dirInfo.GetFiles(template + ".*");
				foreach (System.IO.FileInfo cacheFile in cacheFiles)
				{
					if (cacheFile.CreationTime < expiryTime)
					{
						try
						{
							cacheFile.Delete();
						}
						catch (System.Exception)
						{
							// ignore any problems
						}
					}
				}
			}
		}

		// Deletes files from the run-time cache that have not been persisted to
		// the cache directory.
		public void DeleteNonPersisted()
		{
			System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(this.cacheDir);
			System.IO.FileInfo[] uriFiles = dirInfo.GetFiles("*.wmsuri");

			// Determine all the persisted keys that have associated cache files.
			System.Collections.ArrayList persistedKeys = new System.Collections.ArrayList();
			foreach (System.IO.FileInfo uriFile in uriFiles)
			{
				string template = System.IO.Path.GetFileNameWithoutExtension(uriFile.FullName);
				System.IO.FileInfo[] cacheFiles = dirInfo.GetFiles(template + ".*");
				foreach (System.IO.FileInfo cacheFile in cacheFiles)
				{
					if (!cacheFile.Extension.Equals(".wmsuri"))
					{
						try
						{
							using (System.IO.StreamReader sr = uriFile.OpenText())
							{
								persistedKeys.Add(sr.ReadToEnd());
							}
						}
						catch (System.Exception)
						{
							// ignore any problems
						}
						break;
					}
				}
			}

			// Now delete all the files in the current cache that do not have a persisted key.
			foreach (string key in this.fileList.Keys)
			{
				if (persistedKeys.Contains(key))
					continue;

				deleteFile(this.fileList[key] as string);
			}
		}
	}
}