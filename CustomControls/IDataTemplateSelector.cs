using System;
using Xamarin.Forms;

namespace BookSync.CustomControls
{
    public interface IDataTemplateSelector
    {
        DataTemplate SelectTemplate(object view, object dataItem);
    }
}
