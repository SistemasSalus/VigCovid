using iTextSharp.text;
using iTextSharp.text.pdf;
using System;

namespace NetPdf
{
    public class pdfPage : PdfPageEventHelper
    {
        /** The template with the total number of pages. */
        private PdfTemplate templateNumPage;

        // This is the contentbyte object of the writer
        private PdfContentByte cb;

        // this is the BaseFont we are going to use for the header / footer
        private BaseFont bf = null;

        // This keeps track of the creation time
        private DateTime PrintTime = DateTime.Now;

        //I create a font object to use within my footer
        protected Font footer
        {
            get
            {
                // create a basecolor to use for the footer font, if needed.
                BaseColor grey = new BaseColor(128, 128, 128);
                Font font = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, grey);
                return font;
            }
        }

        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            templateNumPage = writer.DirectContent.CreateTemplate(50, 50);
            cb = writer.DirectContent;
        }

        //override the OnStartPage event handler to add our header
        public override void OnStartPage(PdfWriter writer, Document doc)
        {
            ////I use a PdfPtable with 1 column to position my header where I want it
            //PdfPTable headerTbl = new PdfPTable(1);

            ////set the width of the table to be the same as the document
            //headerTbl.TotalWidth = doc.PageSize.Width;

            ////I use an image logo in the header so I need to get an instance of the image to be able to insert it. I believe this is something you couldn't do with older versions of iTextSharp
            //Image logo = Image.GetInstance(@"Resources\Logo-Laboral-Medical1.jpg");
            ////Image logo = Image.GetInstance(HttpContext.Current.Server.MapPath("/images/logo.jpg"));

            ////I used a large version of the logo to maintain the quality when the size was reduced. I guess you could reduce the size manually and use a smaller version, but I used iTextSharp to reduce the scale. As you can see, I reduced it down to 7% of original size.
            //logo.ScalePercent(5f);

            ////create instance of a table cell to contain the logo
            //PdfPCell cell = new PdfPCell(logo);

            //cell.VerticalAlignment = Element.ALIGN_TOP;

            ////align the logo to the right of the cell
            //cell.HorizontalAlignment = Element.ALIGN_CENTER;

            ////add a bit of padding to bring it away from the right edge
            //cell.PaddingLeft = 20;
            //cell.PaddingTop = -10;

            ////remove the border
            //cell.Border = PdfPCell.NO_BORDER;

            ////Add the cell to the table
            //headerTbl.AddCell(cell);

            ////write the rows out to the PDF output stream. I use the height of the document to position the table. Positioning seems quite strange in iTextSharp and caused me the biggest headache.. It almost seems like it starts from the bottom of the page and works up to the top, so you may ned to play around with this.
            ////headerTbl.WriteSelectedRows(0, -1, 0, (doc.PageSize.Height - 10), writer.DirectContent);
            //headerTbl.WriteSelectedRows(0, -1, 0, (doc.PageSize.Height - 0), writer.DirectContent);
        }

        //override the OnPageEnd event handler to add our footer
        public override void OnEndPage(PdfWriter writer, Document doc)
        {
            ////I use a PdfPtable with 2 columns to position my footer where I want it
            //PdfPTable footerTbl = new PdfPTable(2);

            ////set the width of the table to be the same as the document
            //footerTbl.TotalWidth = doc.PageSize.Width;

            ////Center the table on the page
            //footerTbl.HorizontalAlignment = Element.ALIGN_CENTER;

            String text = string.Format("Página {0} de ", writer.PageNumber);

            float len = bf.GetWidthPoint(text, 8);

            Rectangle pageSize = doc.PageSize;

            cb.SetRGBColorFill(128, 128, 128);

            cb.BeginText();
            cb.SetFontAndSize(bf, 8);
            cb.SetTextMatrix(pageSize.GetLeft(40), pageSize.GetBottom(30));
            cb.ShowText(text);
            cb.EndText();

            cb.AddTemplate(templateNumPage, pageSize.GetLeft(40) + len, pageSize.GetBottom(30));

            cb.BeginText();
            cb.SetFontAndSize(bf, 8);
            cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT,
                "Salus Laboris",//"Impreso El " + PrintTime.ToString(),
                pageSize.GetRight(40),
                pageSize.GetBottom(30), 0);
            cb.EndText();

            //// Page number

            //String pageNumber = string.Format("Página {0} de", writer.PageNumber);

            ////Create a paragraph that contains the footer text
            //Paragraph para = new Paragraph(pageNumber, footer);

            ////add a carriage return
            //para.Add(Environment.NewLine);
            //para.Add("Más Texto de Píe de Página");

            ////create a cell instance to hold the text
            //PdfPCell cell = new PdfPCell(para);

            ////set cell border to 0
            //cell.Border = PdfPCell.NO_BORDER;

            ////add some padding to bring away from the edge
            //cell.PaddingLeft = 10;

            ////add cell to table
            //footerTbl.AddCell(cell);

            //cell = new PdfPCell(Image.GetInstance(templateNumPage));
            ////set cell border to 0
            //cell.Border = PdfPCell.NO_BORDER;

            ////add cell to table
            //footerTbl.AddCell(cell);

            ////create new instance of Paragraph for 2nd cell text
            //para = new Paragraph("Algun Texto de la Segunda Celda", footer);

            ////create new instance of cell to hold the text
            //cell = new PdfPCell(para);

            ////align the text to the right of the cell
            //cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            ////set border to 0
            //cell.Border = PdfPCell.NO_BORDER;

            //// add some padding to take away from the edge of the page
            //cell.PaddingRight = 10;

            ////add the cell to the table
            //footerTbl.AddCell(cell);

            //write the rows out to the PDF output stream.
            //footerTbl.WriteSelectedRows(0, -1, 0, (doc.BottomMargin + 10), writer.DirectContent);
        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            //ColumnText.ShowTextAligned(templateNumPage, Element.ALIGN_LEFT, new Phrase((writer.PageNumber - 1).ToString()), 0, (document.BottomMargin + 10), 0);
            templateNumPage.BeginText();
            templateNumPage.SetFontAndSize(bf, 8);
            templateNumPage.SetTextMatrix(0, 0);
            templateNumPage.ShowText("" + (writer.PageNumber - 1));
            templateNumPage.EndText();
        }
    }
}