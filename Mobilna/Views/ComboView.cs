using System;
using System.Linq;
using Android.Content;
using Android.Runtime;
using Android.Text;
using Android.Util;
using Android.Views;
using Java.Lang;
using MvvmCross.Platforms.Android.Binding.Views;

namespace Mobilna.Views
{
    [Register("mobilna.custom.comboview")]
    [Preserve(AllMembers = true)]
    public class ComboView : MvxAutoCompleteTextView
    {
        public ComboView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            KeyListener = null;
        }
        public ComboView(Context context, IAttributeSet attrs, MvxFilteringAdapter adapter) : base(context, attrs, adapter)
        {
            KeyListener = null;
        }

        protected ComboView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            KeyListener = null;
        }

        protected override void OnFinishInflate()
        {
            base.OnFinishInflate();
            if (Enabled)
            {
                Adapter.FilterPredicate = (o, s) => true;
            }
        }
    }
}