/* Author:			Steven Ma
 * Filename:		Elevens.cs
 * Project Name:	Elevens
 * Creation Date:	September 12th, 2018
 * Modified Date:	September 20th, 2018
 * Description:		Recreates the card game, "Elevens", in graphical form.
 */
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Elevens
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Elevens : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		//Stores the mouse state
		MouseState mouse;
		MouseState prevMouse;

		//Stores all the game states and the current one
		enum GameStates { WinCondition, LoseCondition, InGame };
		GameStates currentGameState = GameStates.InGame;

		//Stores the random object
		Random rng = new Random();

		//Stores the statements made when game is won or loss
		const string WIN_TEXT = "You Win!";
		const string LOSE_TEXT = "You Lose!";
		const string PLAY_AGAIN_TEXT = "PLAY AGAIN?";

		//Stores the positions of text
		Vector2 playAgainLoc = new Vector2(380, 277);
		Vector2 conditionText = new Vector2(415, 164);

		//Stores the screen width and height
		int screenWidth;
		int screenHeight;

		//Stores the max number of cards and shuffles
		const int MAX_CARDS = 52;
		const int NUMBER_OF_SHUFFLES = 1000;
		const int ELEVENS = 11;

		//Stores the text font
		SpriteFont countText;
		SpriteFont conditionTextFont;
		SpriteFont pileNumberTxtFont;

		//Stores the background and its location
		Texture2D background;
		Rectangle bgLoc;

		//Stores the selected cards itself
		Card cardSelected1;
		Card cardSelected2;

		//The back of the card
		Texture2D cardBackImg;
		Rectangle cardBackLoc;
		Texture2D mouseCursor;
		Rectangle mouseCursorLoc;

		//Stores the image of the card faces
		Texture2D cardFaces;

		//Stores the cards and its information that are used during gameplay
		List<Card> deck = new List<Card>();
		Point cardTableOffset = new Point(200, 170);
		Point[] selectedCardsPos = new Point[2];
		bool isFirstCardSelected;
		Card[,] tableCards = new Card[6, 2];
		bool isDeckCreated;
		bool isCardPlaced;
		int cardsRemaining;

		//Stores all the pile numbers
		int[,] pileNumber = new int[6, 2];

		//Stores information on each individual card itself
		Card[] cards = new Card[MAX_CARDS];
		Rectangle[] cardSourceRectangles = new Rectangle[MAX_CARDS];
		Point cardSheetSize = new Point(13, 4);

		//Stores the background music and sound effects
		Song bgMusic;
		SoundEffectInstance cardFlipInstance;
		SoundEffectInstance errorInstance;
		SoundEffectInstance winInstance;
		SoundEffectInstance loseInstance;

		//Stores the yes and no buttons
		Button yesButton;
		Button noButton;

		//Stores the volume buttons and its state
		bool isVolOn = true;
		Button onVolBtn;
		Button offVolBtn;

		//Stores the exit button
		Button exit;

		//Stores the error handling when illegal moves are made
		bool isAddError;
		bool isFaceError;
		bool isPlusFaceError;

		public Elevens()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			// TODO: Add your initialization logic here

			//Sets pile numbers to one
			for (int i = 0; i < pileNumber.GetLength(1); i++)
			{
				for (int j = 0; j < pileNumber.GetLength(0); j++)
				{
					pileNumber[j, i] = 1;
				}
			}

			//Loops the background music
			MediaPlayer.IsRepeating = true;

			//Sets the resolution of the game
			graphics.PreferredBackBufferWidth = 1000;
			graphics.PreferredBackBufferHeight = 600;
			graphics.ApplyChanges();

			//Stores the resolution of the game into variables
			screenWidth = graphics.GraphicsDevice.Viewport.Width;
			screenHeight = graphics.GraphicsDevice.Viewport.Height;

			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

			// TODO: use this.Content to load your game content here

			//Loads in the text font
			countText = Content.Load<SpriteFont>("SpriteFonts/File");
			conditionTextFont = Content.Load<SpriteFont>("SpriteFonts/DefaultFont");
			pileNumberTxtFont = Content.Load<SpriteFont>("SpriteFonts/PileNumber");

			//Loads the background image and its location
			background = Content.Load<Texture2D>("Graphics/Table");
			bgLoc = new Rectangle(0, 0, screenWidth, screenHeight);
			mouseCursor = Content.Load<Texture2D>("Graphics/pointer");

			//Loads the card image and it's location
			cardBackImg = Content.Load<Texture2D>("Graphics/CardBack");
			cardBackLoc = new Rectangle(0, 0, cardBackImg.Width, cardBackImg.Height);

			//Loads the 52 card images
			cardFaces = Content.Load<Texture2D>("Graphics/CardFaces");
			cardSourceRectangles = GetCardRectangles(cardFaces);

			//Loads and plays the background music
			bgMusic = Content.Load<Song>("Music/music");
			MediaPlayer.Play(bgMusic);

			//Loads sound effects
			SoundEffect cardFlip = Content.Load<SoundEffect>("SoundEffects/Flip Card SOUND Effect");
			cardFlipInstance = cardFlip.CreateInstance();
			SoundEffect error = Content.Load<SoundEffect>("SoundEffects/Error");
			errorInstance = error.CreateInstance();
			SoundEffect win = Content.Load<SoundEffect>("SoundEffects/Win");
			winInstance = win.CreateInstance();
			SoundEffect lose = Content.Load<SoundEffect>("SoundEffects/Lose");
			loseInstance = lose.CreateInstance();

			//Loads the yes and no buttons
			Texture2D yesBtnImg = Content.Load<Texture2D>("Graphics/yes");
			Rectangle yesBtnLoc = new Rectangle(150, 373, (int)(yesBtnImg.Width * 0.75), (int)(yesBtnImg.Height * 0.75));
			yesButton = new Button(yesBtnImg, yesBtnLoc);
			Texture2D noBtnImg = Content.Load<Texture2D>("Graphics/no");
			Rectangle noBtnLoc = new Rectangle(570, 373, (int)(noBtnImg.Width * 0.75), (int)(noBtnImg.Height * 0.75));
			noButton = new Button(noBtnImg, noBtnLoc);

			//Loads the exit button
			Texture2D exitBtnImg = Content.Load<Texture2D>("Graphics/exit");
			Rectangle exitBtnLoc = new Rectangle(760, 17, (int)(exitBtnImg.Width * 0.3), (int)(exitBtnImg.Height * 0.3));
			exit = new Button(exitBtnImg, exitBtnLoc);

			//Loads buttons for volume control
			Texture2D onVolImg = Content.Load<Texture2D>("Graphics/onVol");
			Rectangle onVolRec = new Rectangle(880, 20, (int)(onVolImg.Width * 0.45), (int)(onVolImg.Height * 0.45));
			onVolBtn = new Button(onVolImg, onVolRec);
			Texture2D offVolImg = Content.Load<Texture2D>("Graphics/offVol");
			Rectangle offVolRec = new Rectangle(880, 20, (int)(offVolImg.Width * 0.45), (int)(offVolImg.Height * 0.45));
			offVolBtn = new Button(offVolImg, offVolRec);

			//Loads the cards itself and places it on the table
			CreateCards();
			MakeDeck();
			ShuffleCard();
			PlaceCards();

			//Checks to see intitally if the game is lose or not
			CheckForWinCondition();
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// game-specific content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			// TODO: Add your update logic here

			//Updates the mouse cursor location and the mouse state
			mouseCursorLoc = new Rectangle((int)mouse.X, (int)mouse.Y, mouseCursor.Width, mouseCursor.Height);
			mouse = Mouse.GetState();

			switch (currentGameState)
			{
				case GameStates.InGame:
					//Updates the cards remaining in deck
					cardsRemaining = deck.Count;

					//On and off button for music
					if (onVolBtn.ButtonClicked(mouse) && isVolOn && prevMouse.LeftButton != ButtonState.Pressed)
					{
						MediaPlayer.Pause();
						isVolOn = false;
					}
					else if (onVolBtn.ButtonClicked(mouse) && !isVolOn && prevMouse.LeftButton != ButtonState.Pressed)
					{
						MediaPlayer.Resume();
						isVolOn = true;
					}
				
					//Exits the game when button is pressed
					if (exit.ButtonClicked(mouse))
						Exit();

					//Checks to see if the cards are laid down
					if (isCardPlaced && !isAddError && !isFaceError && !isPlusFaceError)
					{
						for (int i = 0; i < tableCards.GetLength(1); i++)
						{
							for (int j = 0; j < tableCards.GetLength(0); j++)
							{
								//Checks if a card was clicked on and mouse is not currently clicked
								if (tableCards[j, i].CardClicked(mouse) && prevMouse.LeftButton != ButtonState.Pressed)
								{
									//Checks whether card is a face or not
									if (tableCards[j, i].value > 0)
									{
										//Finds if selected card is not the first selected card and that first card was selected
										if (tableCards[j, i] != cardSelected1 && isFirstCardSelected)
										{								
											//Selects the second card and checks if it adds to eleven
											cardSelected2 = tableCards[j, i];
											selectedCardsPos[1] = new Point(j, i);
											CheckCard();
										}

										//Checks if the card clicked was selected
										if (!isFirstCardSelected)
										{
											//Selects the card and sets the state whether it was clicked or not
											cardSelected1 = tableCards[j, i];
											selectedCardsPos[0] = new Point(j, i);
											isFirstCardSelected = true;
										}
										else
											//Deselects the first card
											isFirstCardSelected = false;
									}
									else 
									{
										//Checks if the pile number on face card is one
										if (pileNumber[j, i] == 1 && !isFirstCardSelected)
										{
											//Deselects any selected cards
											isFirstCardSelected = false;

											//Swaps the card
											tableCards[j, i] = SwapFaceCard(j, i);

											//Checks to see if game is won, lose or neither
											CheckForWinCondition();
										}
										else
										{
											//Checks which is the error that is occurring
											if (isFirstCardSelected)
											{
												//Plays sound, deselects card and sets error to true
												errorInstance.Play();
												isFirstCardSelected = false;
												isPlusFaceError = true;
											}
											else if(pileNumber[j, i] > 1)
											{
												//Plays sound effect and sets face error to true
												errorInstance.Play();
												isFaceError = true;
											}									
										}
									}
								}
							}
						}
					}
					else
					{
						//Checks if mouse is clicked during error
						if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
						{
							//Checks the type of error
							if (isAddError)
								isAddError = false;
							if (isFaceError)
								isFaceError = false;
							if (isPlusFaceError)
								isPlusFaceError = false;
						}
					}
					break;
				case GameStates.WinCondition:
					//Checks whether user pressed any buttons
					if (yesButton.ButtonClicked(mouse) && prevMouse.LeftButton != ButtonState.Pressed)
						Reset();
					else if (noButton.ButtonClicked(mouse) && prevMouse.LeftButton != ButtonState.Pressed)
						Exit();
					break;
				case GameStates.LoseCondition:
					//Checks if user has pressed any buttons
					if (yesButton.ButtonClicked(mouse) && prevMouse.LeftButton != ButtonState.Pressed)
						Reset();
					else if (noButton.ButtonClicked(mouse) && prevMouse.LeftButton != ButtonState.Pressed)
						Exit();
					break;
			}

			//Stores the previous mouse state
			prevMouse = mouse;

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			// TODO: Add your drawing code here
			spriteBatch.Begin();

			//Draws the background
			spriteBatch.Draw(background, bgLoc, Color.White);

			switch (currentGameState)
			{
				case GameStates.InGame:
					//Draws card back image and exit button
					spriteBatch.Draw(cardBackImg, cardBackLoc, Color.White);
					exit.Draw(spriteBatch);
				
					//Draws volume button
					if (isVolOn)
						onVolBtn.Draw(spriteBatch);
					else
						offVolBtn.Draw(spriteBatch);

					//Draws the selected cards if the deck was created already
					if (isDeckCreated)
					{
						for (int i = 0; i < tableCards.GetLength(1); i++)
						{
							for (int j = 0; j < tableCards.GetLength(0); j++)
							{
								//Draws all of the drawn cards into a pile
								tableCards[j, i].cardLoc = new Rectangle(cardTableOffset.X + (j * Card.size.X), cardTableOffset.Y + (i * Card.size.Y), Card.size.X, Card.size.Y);
								spriteBatch.Draw(cardFaces, tableCards[j, i].cardLoc, tableCards[j, i].cardSourceImg, Color.White);

								//Sees if mouse hovers over card
								if (tableCards[j, i].CardHover(mouse) && !isFaceError && !isAddError)
								{
									//Highlights the card hovered
									spriteBatch.Draw(cardFaces, tableCards[j, i].cardLoc, tableCards[j, i].cardSourceImg, Color.LightYellow);

									//Displays the number of cards in a pile
									if (i < 1)
										spriteBatch.DrawString(pileNumberTxtFont, "Pile Number:\n" + Convert.ToString(pileNumber[j, i]), new Vector2(tableCards[j, i].cardLoc.X, tableCards[j, i].cardLoc.Y - 40), Color.Red);
									else
										spriteBatch.DrawString(pileNumberTxtFont, "Pile Number:\n" + Convert.ToString(pileNumber[j, i]), new Vector2(tableCards[j, i].cardLoc.X, tableCards[j, i].cardLoc.Y + Card.size.Y + 15), Color.Red);
								}

							}
						}

						//Checks if any cards are selected (can be only one)
						if (cardSelected1 != null && isFirstCardSelected)
						{
							//Highlights the card itself
							spriteBatch.Draw(cardFaces, cardSelected1.cardLoc, cardSelected1.cardSourceImg, Color.Yellow);
						}

						//Displays the type of errors to the user
						if (isAddError)
							spriteBatch.DrawString(countText, "ERROR: PAIR DOES NOT ADD TO ELEVEN\n" + "         [LEFT CLICK] TO CONTINUE", new Vector2(150f, 486f), Color.White);
						else if (isFaceError)
							spriteBatch.DrawString(countText, "ERROR: TOO MANY CARDS IN THE PILE\n" + "          [LEFT CLICK] TO CONTINUE", new Vector2(150f, 486f), Color.White);
						else if(isPlusFaceError)
							spriteBatch.DrawString(countText, "ERROR: CAN'T ADD A FACE CARD\n" + "      [LEFT CLICK] TO CONTINUE", new Vector2(180f, 486f), Color.White);
					}

					//Writes the remaining number of cards in the deck
					spriteBatch.DrawString(countText, Convert.ToString(cardsRemaining), new Vector2(25f, 40f), Color.White);
					break;
				case GameStates.WinCondition:
					//Displays the win text and options the player has
					spriteBatch.DrawString(conditionTextFont, WIN_TEXT, conditionText, Color.Gold);
					spriteBatch.DrawString(conditionTextFont, PLAY_AGAIN_TEXT, playAgainLoc, Color.AliceBlue);
					yesButton.Draw(spriteBatch);
					noButton.Draw(spriteBatch);
					break;
				case GameStates.LoseCondition:
					//Displays the lose text and options the player has
					spriteBatch.DrawString(conditionTextFont, LOSE_TEXT, conditionText, Color.Red);
					spriteBatch.DrawString(conditionTextFont, PLAY_AGAIN_TEXT, playAgainLoc, Color.AliceBlue);
					yesButton.Draw(spriteBatch);
					noButton.Draw(spriteBatch);
					break;
			}

			//Draws mouse cursor
			spriteBatch.Draw(mouseCursor, mouseCursorLoc, Color.White);

			spriteBatch.End();

			base.Draw(gameTime);
		}

		/// <summary>
		/// Creates the cards for the game
		/// </summary>
		private void CreateCards()
		{

			//Stores the value of the card
			int value = 0;

			for (int i = 0; i < cards.Length; i++)
			{
				//Increments the card value
				value++;

				//Checks to see if the card value is greater than 13
				if (value > 13)
				{
					//Resets the value back to one
					value = 1;
				}

				//Creates the card and stores it 
				cards[i] = new Card(value, cardSourceRectangles[i], i);

			}

		}

		/// <summary>
		/// Places the cards into 12 sections
		/// </summary>
		private void PlaceCards()
		{

			//Places the cards into the piles in front
			for (int i = 0; i < tableCards.GetLength(1); i++)
			{
				for (int j = 0; j < tableCards.GetLength(0); j++)
				{
					tableCards[j, i] = DrawCard();
				}
			}

			//Sets whether the cards were layed down onto the pile
			isCardPlaced = true;

		}

		/// <summary>
		/// Checks if the game is a lose condition
		/// </summary>
		/// <returns></returns>
		private bool CheckElevensCondition()
		{
	
			//Stores the values of the cards
			int[] listOfValues = new int[tableCards.GetLength(0) * tableCards.GetLength(1)];

			//Tracks the index of the list of values
			int index = 0;

			//Gets and stores the values of the card pile
			for (int i = 0; i < tableCards.GetLength(1); i++)
			{
				for (int j = 0; j < tableCards.GetLength(0); j++)
				{
					//Stores the value and increments index value
					if (tableCards[j, i].value != 0)
						listOfValues[index] = tableCards[j, i].value;
					index++;
				}
			}

			//Sorts list of values from smallest to largest
			Array.Sort(listOfValues);

			//Stores the minimum and maxmimum indices
			int l = 0;
			int r = listOfValues.Length - 1;

			//Checks if the any pairs sums up to eleven
			while (l < r)
			{
				if (listOfValues[l] + listOfValues[r] == ELEVENS)
					return true;
				else if (listOfValues[l] + listOfValues[r] < ELEVENS)
					l++;
				else
					r--;
			}

			return false;
		}

		/// <summary>
		/// Checks whether or not pile with face is still one
		/// </summary>
		private bool CheckForFaceFlips()
		{

			//Stores the face cards locations
			List<Point> faceCards = new List<Point>();

			//Adds the face cards into the list
			for (int i = 0; i < tableCards.GetLength(1); i++)
			{
				for (int j = 0; j < tableCards.GetLength(0); j++)
				{
					if (tableCards[j, i].value == 0)
						faceCards.Add(new Point(j, i));
				}
			}

			//Checks to see if any face card pile has only one card(itself)
			foreach (Point pileNum in faceCards)
			{
				if (pileNumber[pileNum.X, pileNum.Y] == 1)
					return true;
			}

			return false;
		}
		/// <summary>
		/// Checks to see if the two carda adds to eleven
		/// </summary>
		private void CheckCard()
		{

			//Calculates the sum of the two selected cards
			int total = 0;
			total = cardSelected1.value + cardSelected2.value;

			//Checks to see if it adds to eleven
			if (total == ELEVENS)
			{
				//Updates the pile numbers and draws cards to replace the eleven pair
				pileNumber[selectedCardsPos[0].X, selectedCardsPos[0].Y]++;
				pileNumber[selectedCardsPos[1].X, selectedCardsPos[1].Y]++;
				tableCards[selectedCardsPos[0].X, selectedCardsPos[0].Y] = DrawCard();
				tableCards[selectedCardsPos[1].X, selectedCardsPos[1].Y] = DrawCard();

				//Plays sound effect for flipping card
				cardFlipInstance.Play();

				//Checks whether the new pile are all face cards or not
				CheckForWinCondition();
			}
			else
			{
				//Sets that error has ocurred and plays error sound
				errorInstance.Play();
				isAddError = true;
			}

		}

		/// <summary>
		/// Checks if game is won
		/// </summary>
		/// <returns></returns>
		private bool CheckForAllFaceCards()
		{

			//Stores the sum of the pile
			int sum = 0;

			//Calculates the sum of the cards value
			for (int i = 0; i < tableCards.GetLength(1); i++)
			{
				for (int j = 0; j < tableCards.GetLength(0); j++)
				{
					sum += tableCards[j, i].value;
				}
			}

			//Checks if all the cards are faces
			if (sum == 0)
				return true;
			else
				return false;

		}

		/// <summary>
		/// Makes a deck from the cards itself
		/// </summary>
		private void MakeDeck()
		{

			//Adds the created cards into the deck list
			deck.AddRange(cards);

			//Stores whether deck was made
			isDeckCreated = true;

		}

		/// <summary>
		/// Checks to see if the player lose
		/// </summary>
		private void CheckForWinCondition()
		{

			//Checks whether the entire pile is full of face cards
			if (CheckForAllFaceCards())
			{
				//Plays victory noise and tells user they won
				winInstance.Play();
				currentGameState = GameStates.WinCondition;
			}
			else if (!CheckElevensCondition())
			{
				//Checks if there isn't a face card on its own
				if (!CheckForFaceFlips())
				{
					//Plays lose noise and tells user they lose
					loseInstance.Play();
					currentGameState = GameStates.LoseCondition;
				}
			}

		}

		/// <summary>
		/// Swaps the face card for another card
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		private Card SwapFaceCard(int x, int y)
		{

			//Places the face card on the bottom of the deck and swaps it
			deck.Insert(0, tableCards[x, y]);

			//Plays the sound effect for flipping cards
			cardFlipInstance.Play();

			return DrawCard();

		}

		/// <summary>
		/// Draws a card from the deck
		/// </summary>
		/// <returns></returns>
		private Card DrawCard()
		{

			//Stores the drawn card
			Card drawnCard;

			//Selects the drawn card from deck via removing it
			drawnCard = deck[deck.Count - 1];
			deck.RemoveAt(deck.Count - 1);

			//Returns the drawn card
			return drawnCard;

		}

		/// <summary>
		/// Shuffles the deck itself
		/// </summary>
		private void ShuffleCard()
		{

			//Shuffles the cards many times as defined
			for (int i = 0; i < NUMBER_OF_SHUFFLES; i++)
			{
				//Stores the randomly selected cards
				Card tempHold;
				Card tempHold2;

				//Finds a random card from the deck
				int random1 = rng.Next(0, 52);
				int random2 = rng.Next(0, 52);

				//Swaps the two randomly selected cards positions in the deck
				if(random1!=random2)
				{
					tempHold = deck[random1];
					tempHold2 = deck[random2];
					deck[random1] = tempHold2;
					deck[random2] = tempHold;
				}		
			}

		}

		/// <summary>
		/// Gets the location of each card on the source rectangle(card sheet)
		/// </summary>
		/// <param name="sourceTexture"></param>
		/// <returns></returns>
		private Rectangle[] GetCardRectangles(Texture2D sourceTexture)
		{
			//Stores the card images
			Rectangle[] cards = new Rectangle[MAX_CARDS];

			//Tracks the index of the individual cards
			int index = 0;

			//Cuts up the cards on the cards sheet and stores it into an array
			for (int i = 0; i < cardSheetSize.Y; i++)
			{
				for (int j = 0; j < cardSheetSize.X; j++)
				{
					Rectangle card = new Rectangle(j * Card.size.X, i * Card.size.Y, Card.size.X, Card.size.Y);
					cards[index++] = card;
				}
			}

			//Returns back the card array
			return cards;
		}

		/// <summary>
		/// Resets the game
		/// </summary>
		private void Reset()
		{

			//Resets all bool values
			isCardPlaced = false;
			isDeckCreated = false;
			isFirstCardSelected = false;
			isAddError = false;
			isFaceError = false;

			//Removes all the cards from the deck
			deck.RemoveRange(0, deck.Count);

			//Removes all the cards from the pile
			for (int i = 0; i < tableCards.GetLength(1); i++)
			{
				for (int j = 0; j < tableCards.GetLength(0); j++)
				{
					tableCards[j, i] = null;
					pileNumber[j, i] = 1;
				}
			}

			//Reloads the cards itself and places it on the table
			MakeDeck();
			ShuffleCard();
			PlaceCards();

			//Goes back to the game
			currentGameState = GameStates.InGame;

			//Checks to see intitally if the game is lose or not
			CheckForWinCondition();

		}
	}
}
