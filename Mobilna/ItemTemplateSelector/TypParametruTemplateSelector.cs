using Android;
using Core.Models;
//using MvvmCross.Droid.Support.V7.RecyclerView.ItemTemplates;
using MvvmCross.DroidX.RecyclerView.ItemTemplates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mobilna.ItemTemplateSelector
{
    public class TypParametruTemplateSelector : IMvxTemplateSelector
    {
        public TypParametruTemplateSelector()
        {

        }
        public int ItemTemplateId { set; get; }
        public int GetItemViewType(Object o)
        {
            if (!(o is Parametr p))
                return -1;
            if (p.TypParametrow.TypDanych == "STRING")
                return 1;
            if (p.TypParametrow.TypDanych == "INT")
                return 2;
            if (p.TypParametrow.TypDanych == "ENUM")
                return 3;
            if (p.TypParametrow.TypDanych == "TIME")
                return 4;
            if (p.TypParametrow.TypDanych == "DATE")
                return 5;
            if (p.TypParametrow.TypDanych == "DATETIME")
                return 6;
            if (p.TypParametrow.TypDanych == "FLOAT")
                return 7;

            return -1;

        }
        public int GetItemLayoutId(int viewType)
        {
            if (viewType == 1)
                return Resource.Layout.obiekt_form_text_item;
            if (viewType == 2)
                return Resource.Layout.obiekt_form_int_item;
            if (viewType == 3)
                return Resource.Layout.obiekt_form_enum_item;
            if (viewType == 4)
                return Resource.Layout.obiekt_form_time_item;
            if (viewType == 5)
                return Resource.Layout.obiekt_form_date_item;
            if (viewType == 6)
                return Resource.Layout.obiekt_form_datetime_item;
            if (viewType == 7)
                return Resource.Layout.obiekt_form_float_item;

            return ItemTemplateId;
        }

    }
}
