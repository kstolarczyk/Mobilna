using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Text;
using Android.Util;
using Google.Android.Material.TextField;

namespace Mobilna.Views
{
    [Register("mobilna.custom.datetimepicker")]
    [Preserve(AllMembers = true)]
    public class DateTimePickerView : TextInputEditText
    {
        protected DateTimePickerView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            SetupControl();
        }

        public DateTimePickerView(Context context) : base(context)
        {
            SetupControl();
        }

        public DateTimePickerView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            SetupControl();
        }

        public DateTimePickerView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            SetupControl();
        }

        protected void SetupControl()
        {
            InputType = InputTypes.DatetimeVariationNormal;
            KeyListener = null;
            Clickable = true;
            FocusChange += ShowCalendar;
        }

        private void ShowCalendar(object sender, EventArgs e)
        {
            if (!HasFocus) return;
            var dateTime = DateTime.Now;
            var picker = new DatePickerDialog(Context!, OnDateSet, dateTime.Year, dateTime.Month, dateTime.Day);
            picker.Show();
        }

        private void OnDateSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            var dateTime = DateTime.Now;
            _dateSet = e.Date;
            var picker = new TimePickerDialog(Context!, OnTimeSet, dateTime.Hour, dateTime.Minute, true);
            picker.Show();
        }

        private void OnTimeSet(object sender, TimePickerDialog.TimeSetEventArgs e)
        {
            _dateSet = _dateSet.AddHours(e.HourOfDay);
            _dateSet = _dateSet.AddMinutes(e.Minute);
            Text = _dateSet.ToString("dd.MM.yyyy hh:mm");
        }

        private DateTime _dateSet;
    }
}