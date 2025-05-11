using CollectorsApp.Models;

namespace CollectorsApp.Repository.Interfaces
{
    public interface IImagePathRepository
    {
        Task<IEnumerable<ImagePath>> GetImagePaths();
        Task<IEnumerable<ImagePath>> GetImagePathsByItemId(int itemId);
        Task PostImagePath(ImagePath imagePath);
        Task<ImagePath> GetImagePathByImageId(int id);
        Task UpdateImagePath(ImagePath imagePath, int id);
        Task DeleteImagePath(int id);

    }
}
