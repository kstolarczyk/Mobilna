using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Text;
using Android.Util;
using Android.Widget;
using Google.Android.Material.TextField;

namespace Mobilna.Views
{
    [Register("mobilna.custom.datepicker")]
    [Preserve(AllMembers = true)]
    public class DatePickerView : TextInputEditText
    {
        protected void SetupControl()
        {
            InputType = InputTypes.DatetimeVariationDate;
            KeyListener = null;
            Clickable = true;
            FocusChange += ShowCalendar;
        }

        private void ShowCalendar(object sender, EventArgs e)
        {
            if (!HasFocus) return;
            var dateTIme = DateTime.Now;
            var picker = new DatePickerDialog(Context!, OnDateSet, dateTIme.Year, dateTIme.Month, dateTIme.Day);
            picker.Show();
        }

        private void OnDateSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            Text = e.Date.ToString("dd.MM.yyyy");
        }

        protected DatePickerView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            SetupControl();
        }

        public DatePickerView(Context context) : base(context)
        {
            SetupControl();
        }

        public DatePickerView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            SetupControl();
        }

        public DatePickerView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            SetupControl();
        }
    }
}