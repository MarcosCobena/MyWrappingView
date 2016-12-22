namespace MyWrappingView
{
	using System;
	using System.Linq;
	using CoreGraphics;
	using Foundation;
	using UIKit;

	[Register("MyWrappingView")]
	public class MyWrappingView : UIView
	{
		float _preferredMaxLayoutWidth;

		public MyWrappingView(IntPtr handle) : base(handle)
        {
			Initialize();
		}

		public MyWrappingView(CGRect frame) : base(frame)
        {
			Initialize();
		}

		public override CGSize IntrinsicContentSize
		{
			get
			{
				if (!Subviews.Any())
					return CGSize.Empty;

				var totalRect = CGRect.Empty;

				EnumerateItemRectsForLayoutWidth(_preferredMaxLayoutWidth,
												 itemRect => totalRect = CGRect.Union(itemRect, totalRect));

				return totalRect.Size;
			}
		}

		public float HorizontalSpacing { get; set; }

		public float VerticalSpacing { get; set; }

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			// Disable animation during rotation, for example
			var wereEnabled = UIView.AnimationsEnabled;

			UIView.AnimationsEnabled = false;

			var enumerator = Subviews.GetEnumerator();

			EnumerateItemRectsForLayoutWidth((float)Bounds.Size.Width,
											 itemRect =>
			{
				if (enumerator.MoveNext())
					(enumerator.Current as UIView).Frame = itemRect;
			});

			UIView.AnimationsEnabled = wereEnabled;
		}

		public void SetPreferredMaxLayoutWidth(float width)
		{
			if (_preferredMaxLayoutWidth.Equals(width))
				return;

			_preferredMaxLayoutWidth = width;

			InvalidateIntrinsicContentSize();
		}

		void Initialize()
		{
			HorizontalSpacing = 0;
			VerticalSpacing = 0;
		}

		void EnumerateItemRectsForLayoutWidth(float layoutWidth, Action<CGRect> block)
		{
			float x = 0, y = 0;

			foreach (var view in Subviews)
			{
				var width = (float)view.Frame.Width;
				var height = (float)view.Frame.Height;

				if (x > layoutWidth - width)
				{
					y += height + VerticalSpacing;
					x = 0;
				}

				block(new CGRect(x, y, width, height));

				x += width + HorizontalSpacing;
			}
		}
	}
}
