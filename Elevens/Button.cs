using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Elevens
{
	class Button
	{
		//Stores information on button dimensions
		private Texture2D image;
		private Rectangle bounds;

		/// <summary>
		/// Initializes a new instance of button
		/// </summary>
		/// <param name="Image"></param>
		/// <param name="Bounds"></param>
		public Button(Texture2D Image, Rectangle Bounds)
		{

			image = Image;
			bounds = Bounds;

		}

		/// <summary>
		/// Checks to see if the button has been clicked
		/// </summary>
		/// <param name="mouse"></param>
		/// <returns></returns>
		public bool ButtonClicked(MouseState mouse)
		{

			//Checks to see if mouse and button are in boundaries and the mouse is clicked
			if (bounds.X <= mouse.X && mouse.X <= (bounds.X + bounds.Width) &&
				bounds.Y <= mouse.Y && (bounds.Y + bounds.Width) >= mouse.Y &&
				mouse.LeftButton == ButtonState.Pressed)
			{
				return true;
			}
			else
			{
				return false;
			}

		}

		/// <summary>
		/// Draws the button
		/// </summary>
		/// <param name="spriteBatch"></param>
		public void Draw(SpriteBatch spriteBatch)
		{

			//Draws the button itself on screen
			spriteBatch.Draw(image, bounds, Color.White);

		}

	}
}
