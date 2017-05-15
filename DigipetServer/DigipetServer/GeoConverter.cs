using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigipetServer
{
    class GeoConverter
    {
        private static int tileSize = 256;
        private static int earthRadius = 6378137;
        private static float initialResolution = 2 * (float)Math.PI * earthRadius / tileSize;
        private static float originShift = 2 * (float)Math.PI * earthRadius / 2;

        public static float[] GeoCoorToMercatorProjection(float latitude, float longitude)
        {
            //float n = Mathf.Pow(2, zoom);
            //float x = n * ((longitude + 180) / 360);
            //float y = n * (1 - (Mathf.Log(Mathf.Tan(latitude * Mathf.PI / 180) + (1 / Mathf.Cos(latitude * Mathf.PI / 180))) / Mathf.PI)) / 2;

            float x = (float)(longitude * originShift / 180);
            float y = (float)(Math.Log(Math.Tan((90 + latitude) * Math.PI / 360)) / (Math.PI / 180));
            y = (float)(y * originShift / 180);

            float[] mercatorProjectionLocation = new float[2];
            mercatorProjectionLocation[0] = x;
            mercatorProjectionLocation[1] = y;

            return mercatorProjectionLocation;
        }

        // Convert coordinate system based on mercator projection to pixel coordinate system
        public static float[] MercatorProjectionToPixel(float[] mercatorProjection, int zoom)
        {
            float res = (float)(initialResolution / (Math.Pow(2, zoom)));
            float x = (mercatorProjection[0] + originShift) / res;
            float y = (-1 * mercatorProjection[1] + originShift) / res;

            float[] pixelLocation = new float[2];
            pixelLocation[0] = x;
            pixelLocation[1] = y;

            return pixelLocation;
        }

        // Convert pixel coordinate system to tile coordinate system
        public static int[] PixelToTileCoordinate(float[] pixel)
        {
            int x = (int)(Math.Ceiling(pixel[0] / tileSize) - 1);
            int y = (int)(Math.Ceiling(pixel[1] / tileSize) - 1);

            int[] tileLocation = new int[2];
            tileLocation[0] = x;
            tileLocation[1] = y;

            return tileLocation;
        }
    }
}
