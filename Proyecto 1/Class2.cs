using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using A = DocumentFormat.OpenXml.Drawing;

namespace Proyecto_1
{
    public static class PowerPointHelper
    {
        public static void CrearPresentacion(string contenido, string path)
        {
            using (PresentationDocument presentationDoc = PresentationDocument.Create(path, DocumentFormat.OpenXml.PresentationDocumentType.Presentation))
            {
                var presPart = presentationDoc.AddPresentationPart();
                presPart.Presentation = new Presentation();

                var slidePart = presPart.AddNewPart<SlidePart>();
                slidePart.Slide = new Slide(new CommonSlideData(new ShapeTree()));

                presPart.Presentation.AppendChild(new SlideIdList(
                    new SlideId() { Id = 256U, RelationshipId = presPart.GetIdOfPart(slidePart) }));
            }
        }
    }
}
