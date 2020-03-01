using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Elevens
{
	class Card
	{
		//Stores the value of the card
		public readonly int value;

		//Stores the location of the card on the card faces sheet
		public readonly Rectangle cardSourceImg;

		//Stores the position of the card in game
		public Rectangle cardLoc;

		//Stores the card size
		public static readonly Point size = new Point(91, 128);

		/// <summary>
		/// Constructor for a Card object
		/// </summary>
		/// <param name="value"></param>
		/// <param name="cardSourceImg"></param>
		/// <param name="recIndex"></param>
		public Card(int value, Rectangle cardSourceImg, int recIndex)
		{

			//Checks whether the value of the card is over ten
			if(value>10)
			{
				//Sets the value to zero
				value = 0;
			}

			//Stores in the data passed in via the constructor
			this.value = value;
			this.cardSourceImg = cardSourceImg;

		}

		/// <summary>
		/// Checks to see if the button has been clicked
		/// </summary>
		/// <param name="mouse"></param>
		/// <returns></returns>
		public bool CardClicked(MouseState mouse)
		{

			//Checks to see if mouse and button are in boundaries and the mouse is clicked
			if (cardLoc.X <= mouse.X && mouse.X <= (cardLoc.X + cardLoc.Width) &&
				cardLoc.Y <= mouse.Y && (cardLoc.Y + cardLoc.Width) >= mouse.Y &&
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
		/// Checks to see is player hovers over card
		/// </summary>
		/// <param name="mouse"></param>
		/// <returns></returns>
		public bool CardHover(MouseState mouse)
		{

			//Checks to see if mouse and button are in boundaries and the mouse is clicked
			if (cardLoc.X <= mouse.X && mouse.X <= (cardLoc.X + cardLoc.Width) &&
				cardLoc.Y <= mouse.Y && (cardLoc.Y + cardLoc.Width) >= mouse.Y)
			{
				return true;
			}
			else
			{
				return false;
			}

		}
	}
}
