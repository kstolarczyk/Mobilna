using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Text;
using Android.Util;
using Google.Android.Material.TextField;

namespace Mobilna.Views
{
    [Register("mobilna.custom.timepicker")]
    [Preserve(AllMembers = true)]
    public class TimePickerView : TextInputEditText
    {
        protected TimePickerView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            SetupControl();
        }

        public TimePickerView(Context context) : base(context)
        {
            SetupControl();
        }

        public TimePickerView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            SetupControl();
        }

        public TimePickerView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            SetupControl();
        }

        protected void SetupControl()
        {
            InputType = InputTypes.DatetimeVariationTime;
            KeyListener = null;
            Clickable = true;
            FocusChange += ShowCalendar;
        }

        private void ShowCalendar(object sender, EventArgs e)
        {
            if (!HasFocus) return;
            var dateTime = DateTime.Now;
            var picker = new TimePickerDialog(Context!, OnTimeSet, dateTime.Hour, dateTime.Minute, true);
            picker.Show();
        }

        private void OnTimeSet(object sender, TimePickerDialog.TimeSetEventArgs e)
        {
            Text = $"{e.HourOfDay.ToString().PadLeft(2, '0')}:{e.Minute.ToString().PadLeft(2, '0')}";
        }
    }
}