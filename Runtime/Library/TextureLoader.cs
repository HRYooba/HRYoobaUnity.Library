using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using UnityEngine;

namespace HRYooba.Library
{
    public static class TextureLoader
    {
        public static Texture2D[] LoadTextures(string directPath)
        {
            if (Directory.Exists(directPath))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directPath);
                var fileInfos = directoryInfo.GetFiles("*").Where(_ => _.Name.EndsWith(".jpg") || _.Name.EndsWith(".png"));

                if (fileInfos.Count() > 0)
                {
                    var textures = new Texture2D[fileInfos.Count()];
                    for (var i = 0; i < fileInfos.Count(); i++)
                    {
                        var path = directPath + "\\" + fileInfos.ToArray()[i].Name;
                        if (File.Exists(path))
                        {
                            var bytes = File.ReadAllBytes(path);
                            textures[i] = new Texture2D(2, 2);
                            textures[i].LoadImage(bytes);
                            Debug.Log("Load texture: " + path);
                        }
                    }

                    return textures;
                }
                else
                {
                    Debug.LogError("Not exists textures: " + directPath);
                    return null;
                }
            }
            else
            {
                Debug.LogError("Not exists path: " + directPath);
                return null;
            }
        }

        public static async Task<Texture2D[]> LoadTexturesAsync(string directPath, CancellationToken cancellationToken)
        {
            if (Directory.Exists(directPath))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directPath);
                var fileInfos = directoryInfo.GetFiles("*").Where(_ => _.Name.EndsWith(".jpg") || _.Name.EndsWith(".png"));

                if (fileInfos.Count() > 0)
                {
                    var textures = new Texture2D[fileInfos.Count()];
                    for (var i = 0; i < fileInfos.Count(); i++)
                    {
                        var path = directPath + "\\" + fileInfos.ToArray()[i].Name;
                        if (File.Exists(path))
                        {
                            var bytes = await File.ReadAllBytesAsync(path, cancellationToken);
                            textures[i] = new Texture2D(2, 2);
                            textures[i].LoadImage(bytes);
                            Debug.Log("Load texture: " + path);
                        }
                    }

                    return textures;
                }
                else
                {
                    Debug.LogError("Not exists textures: " + directPath);
                    return null;
                }
            }
            else
            {
                Debug.LogError("Not exists path: " + directPath);
                return null;
            }
        }
    }
}