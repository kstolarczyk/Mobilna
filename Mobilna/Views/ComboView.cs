using System;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using MvvmCross.Platforms.Android.Binding.Views;

namespace Mobilna.Views
{
    [Register("mobilna.custom.comboview")]
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
    }
}