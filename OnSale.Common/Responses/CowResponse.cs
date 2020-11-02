using OnSale.Common.Entities;
using System.Collections.Generic;
using System.Linq;

namespace OnSale.Common.Responses
{
    public class CowResponse
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public UserResponse Owner { get; set; }

        public Fair Fair { get; set; }

        public City city { get; set; }

        public decimal Price { get; set; }

        public bool IsActive { get; set; }

        public ICollection<CowImage> ProductImages { get; set; }

        public int ProductImagesNumber => ProductImages == null ? 0 : ProductImages.Count;

        public string ImageFullPath => ProductImages == null || ProductImages.Count == 0
            ? $"https://onsalel.azurewebsites.net/images/noimage.png"
            : ProductImages.FirstOrDefault().ImageFullPath;

        public ICollection<QualificationResponse> Qualifications { get; set; }

        public int ProductQualifications => Qualifications == null ? 0 : Qualifications.Count;

        public float Qualification => Qualifications == null || Qualifications.Count == 0 ? 0 : Qualifications.Average(q => q.Score);
    }

}
