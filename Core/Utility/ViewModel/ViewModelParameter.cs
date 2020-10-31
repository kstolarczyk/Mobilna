namespace Core.Utility.ViewModel
{
    public class ViewModelParameter<T>
    {
        public ViewModelParameter() {}
        public ViewModelParameter(T value)
        {
            Value = value;
        }
        public T Value { get; set; }
    }
}