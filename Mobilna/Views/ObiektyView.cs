using Acr.UserDialogs;
using Android.App;
using Android.OS;
using Android.Views;
using Core.Interactions;
using Core.Models;
using Core.ViewModels;
using MvvmCross.Base;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Views;
using Xamarin.Essentials;

namespace Mobilna.Views
{
    [Activity(Label = "@string/obiekty_title")]
    public class ObiektyView : MvxActivity<ObiektyViewModel>
    {
        private MvxRecyclerView _listView;
        private ContextMenuInteraction<Obiekt> _contextMenuInteraction;

        private void OnInteractionRequested(object sender, MvxValueEventArgs<ContextMenuInteraction<Obiekt>> e)
        {
            _contextMenuInteraction = e.Value;
            OpenContextMenu(_listView);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            // Platform.Init(this, bundle);
            SetContentView(Resource.Layout.obiekty_list);
            _listView = FindViewById<MvxRecyclerView>(Resource.Id.obiekty_recyclerview);
            RegisterForContextMenu(_listView);
            ViewModel.ContextMenuInteraction.Requested += OnInteractionRequested;
        }

        public override void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            if (v.Id != Resource.Id.obiekty_recyclerview) return;
            menu.Add(Menu.None, 0, 0, Resource.String.szczegoly_obiekty_view);
            menu.Add(Menu.None, 1, 1, Resource.String.edytuj_obiekty_view);
            menu.Add(Menu.None, 2, 2, Resource.String.usun_obiekty_view);
            menu.SetHeaderTitle(_contextMenuInteraction.CurrentObiekt.Nazwa);
            if (_contextMenuInteraction.CurrentObiekt.User != null) return;
            menu.GetItem(1)?.SetEnabled(false);
            menu.GetItem(2)?.SetEnabled(false);
        }

        public override bool OnContextItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case 0:
                    _contextMenuInteraction.ContextMenuCallback(_contextMenuInteraction.CurrentObiekt, ContextMenuOption.Details);
                    break;
                case 1:
                    _contextMenuInteraction.ContextMenuCallback(_contextMenuInteraction.CurrentObiekt, ContextMenuOption.Edit);
                    break;
                case 2:
                    _contextMenuInteraction.ContextMenuCallback(_contextMenuInteraction.CurrentObiekt, ContextMenuOption.Delete);
                    break;

            }
            return true;
        }

    }
}