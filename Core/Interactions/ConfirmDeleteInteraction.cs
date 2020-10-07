using System;
using Core.Models;

namespace Core.Interactions
{
    public class ConfirmDeleteInteraction<T>
    {
        public Action<T, bool> DeleteCallback { get; set; }
    }
}