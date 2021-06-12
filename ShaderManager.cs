using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Journey_Of_The_Ship
{
    public class ShaderManager      //In this class there's references to all shader effects needed
    {
        public static Effect blackHoleShader;

        public Effect activeScreenShader;

        private float time = 0f;

        public void ActivateBlackHoleShader(float shaderStrength, Vector2 position, float shaderDistance)
        {
            activeScreenShader = blackHoleShader;
            activeScreenShader.Parameters["shaderStrength"].SetValue(shaderStrength);
            activeScreenShader.Parameters["distortionCenter"].SetValue(position);
            activeScreenShader.Parameters["distortionDistance"].SetValue(shaderDistance);
        }

        public void Update()
        {
            if (activeScreenShader != null)
            {
                time += 0.01f;
                ////activeScreenShader.Parameters["time"].SetValue(time);
                activeScreenShader.Parameters["resolution"].SetValue(new Vector2(Main.desiredResolutionWidth, Main.desiredResolutionHeight));
            }
            else
            {
                time = 0f;
            }
        }

        public void DisableScreenShaders()
        {
            activeScreenShader = null;
        }
    }
}
