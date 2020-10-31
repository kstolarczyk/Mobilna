using Acr.UserDialogs;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Core.Interactions;
using Core.Models;
using Core.ViewModels;
using MvvmCross.Base;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Views;
using MvvmCross.Platforms.Android.Views.Fragments;
using Xamarin.Essentials;

namespace Mobilna.Views
{
    [Register("mobilna.custom.fragments.ObiektyFragment")]
    public class ObiektyView : MvxFragment<ObiektyViewModel>
    {
        private MvxRecyclerView _listView;
        private ContextMenuInteraction<Obiekt> _contextMenuInteraction;

        private void OnInteractionRequested(object sender, MvxValueEventArgs<ContextMenuInteraction<Obiekt>> e)
        {
            _contextMenuInteraction = e.Value;
            Activity.OpenContextMenu(_listView);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var ignore = base.OnCreateView(inflater, container, savedInstanceState);
            var view = this.BindingInflate(Resource.Layout.obiekty_recyclerview, null);
            _listView = view.FindViewById<MvxRecyclerView>(Resource.Id.obiekty_recyclerview_list);
            RegisterForContextMenu(_listView);
            ViewModel.ContextMenuInteraction.Requested += OnInteractionRequested;
            return view;
        }

        public override void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            if (v.Id != Resource.Id.obiekty_recyclerview_list) return;
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