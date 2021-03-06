﻿using DynamicDataDisplay.Common;
using System;
using System.Collections.Specialized;

namespace DynamicDataDisplay.ViewportRestrictions
{
	/// <summary>
	/// Represents a collection of <see cref="ViewportRestriction"/>s.
	/// <remarks>
	/// ViewportRestriction that is being added should not be null.
	/// </remarks>
	/// </summary>
	public sealed class RestrictionCollection : D3Collection<ViewportRestriction>
	{
		private readonly Viewport2D viewport;
		public RestrictionCollection(Viewport2D viewport)
		{
			if (viewport == null)
				throw new ArgumentNullException("viewport");

			this.viewport = viewport;
		}

		protected override void OnItemAdding(ViewportRestriction item)
		{
			if (item == null)
				throw new ArgumentNullException("item");
		}

		protected override void OnItemAdded(ViewportRestriction item)
		{
			item.Changed += OnItemChanged;
			ISupportAttachToViewport attachable = item as ISupportAttachToViewport;
			if (attachable != null)
			{
				attachable.Attach(viewport);
			}
		}

		private void OnItemChanged(object sender, EventArgs e)
		{
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		protected override void OnItemRemoving(ViewportRestriction item)
		{
			ISupportAttachToViewport attachable = item as ISupportAttachToViewport;
			if (attachable != null)
			{
				attachable.Detach(viewport);
			}
			item.Changed -= OnItemChanged;
		}

		public DataRect Apply(DataRect oldVisible, DataRect newVisible, Viewport2D viewport)
		{
			DataRect res = newVisible;
			foreach (var restriction in this)
			{
				res = restriction.Apply(oldVisible, res, viewport);
			}
			return res;
		}
	}
}
