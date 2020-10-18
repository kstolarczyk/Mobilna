using System.IO;
using System.Threading.Tasks;
using Android.Content;
using Core;
using Core.Interfaces;
using Mobilna.Views;

namespace Mobilna.Services
{
    public class PhotoPickerService : IPickImageService
    {
        public Task<byte[]> GetImageStreamAsync()
        {
            // Define the Intent for getting images
            Intent intent = new Intent();
            intent.SetType("image/*");
            intent.SetAction(Intent.ActionGetContent);

            // Start the picture-picker activity (resumes in MainActivity.cs)
            ObiektFormView.Instance.StartActivityForResult(
                Intent.CreateChooser(intent, "Select Picture"),
                ObiektFormView.PickImageId);

            // Save the TaskCompletionSource object as a MainActivity property
            ObiektFormView.Instance.PickImageTask = new TaskCompletionSource<byte[]>();

            // Return Task object
            return ObiektFormView.Instance.PickImageTask.Task;
        } 
    }
}