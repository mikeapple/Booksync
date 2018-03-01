using System;
using Xamarin.Forms;

namespace BookSync.CustomControls
{
    public class ScsListView : ListView
    {
        public ScsListView()
        {
            ItemTemplate = new DataTemplate(GetHookedCell);
        }

        Cell GetHookedCell()
        {
            var content = new ViewCell();
            content.BindingContextChanged += OnBindingContextChanged;
            return content;
        }

        public static readonly BindableProperty TemplateSelectorProperty = BindableProperty.Create<ScsListView, IDataTemplateSelector>(p => p.TemplateSelector, null);

        public IDataTemplateSelector TemplateSelector
        {
            get { return (IDataTemplateSelector)GetValue(TemplateSelectorProperty); }
            set { SetValue(TemplateSelectorProperty, value); }
        }


        private void OnBindingContextChanged(object sender, EventArgs e)
        {
            var cell = (ViewCell)sender;
            if (TemplateSelector != null)
            {
                var template = TemplateSelector.SelectTemplate(cell, cell.BindingContext);
                cell.View = ((ViewCell)template.CreateContent()).View;
            }
        }

    }
}
