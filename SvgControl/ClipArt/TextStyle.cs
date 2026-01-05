using System.Windows;

namespace SvgControl
{
    public class TextStyle
    {
        public TextStyle(TextStyle aCopy)
        {
            Copy(aCopy);
        }

        public TextStyle(SVG svg, Shape owner)
        {
            FontFamily = "Arial Unicode MS, Verdana";
            FontSize = 12;
            Fontweight = FontWeights.Normal;
            Fontstyle = FontStyles.Normal;
            TextAlignment = TextAlignment.Left;
            WordSpacing = 0;
            LetterSpacing = 0;
            BaseLineShift = string.Empty;
            if (owner.Parent != null)
                Copy(owner.Parent.TextStyle);
        }

        public string FontFamily { get; set; }
        public double FontSize { get; set; }
        public FontWeight Fontweight { get; set; }
        public FontStyle Fontstyle { get; set; }
        public TextDecorationCollection TextDecoration { get; set; }
        public TextAlignment TextAlignment { get; set; }
        public double WordSpacing { get; set; }
        public double LetterSpacing { get; set; }
        public string BaseLineShift { get; set; }

        public void Copy(TextStyle aCopy)
        {
            if (aCopy == null)
                return;
            FontFamily = aCopy.FontFamily;
            FontSize = aCopy.FontSize;
            Fontweight = aCopy.Fontweight;
            Fontstyle = aCopy.Fontstyle;
            ;
            TextAlignment = aCopy.TextAlignment;
            WordSpacing = aCopy.WordSpacing;
            LetterSpacing = aCopy.LetterSpacing;
            BaseLineShift = aCopy.BaseLineShift;
        }
    }
}