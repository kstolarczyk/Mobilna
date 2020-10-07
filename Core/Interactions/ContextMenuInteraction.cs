using System;
using Core.Models;

namespace Core.Interactions
{
    public class ContextMenuInteraction<T>
    {
        public Action<T, ContextMenuOption> ContextMenuCallback { get; set; }
        public Obiekt CurrentObiekt { get; set; }
    }

    public enum ContextMenuOption
    {
        None,
        Details,
        Edit,
        Delete,
    }
}