using System.Drawing.Imaging;

namespace Snack_Game
{
    public partial class Form1 : Form
    {

        /*###########################################################################
         * ######################   Welcome To Snack Game:   ########################
         ##########################################################################*/

        //snack is like a list or array of some circle => we did not use array because it has a fixed size
        private List<Circle> snack = new List<Circle>();
        //food is a only one circle in a random place 
        private Circle food = new Circle();
        //maxWidth of the Picture box that snack will run on it
        int maxWidth;
        //maxWidth of the Picture box that snack will run on it
        int maxHeight;

        int score;
        int highScore;
        //for food place
        Random rand = new Random();
        //directions:
        bool goLeft, goRight, goUp, goDown;


        public Form1()
        {
            InitializeComponent();
            new Settings();
        }

        private void keyIsDown(object sender, KeyEventArgs e)
        {
            if ( e.KeyCode == Keys.Left && Settings.directions != "right")
            {
                goLeft = true;
            }
            if ( e.KeyCode == Keys.Right && Settings.directions != "left")
            {
                goRight = true;
            }
            if (e.KeyCode == Keys.Up && Settings.directions != "down")
            {
                goUp = true;
            }
            if ( e.KeyCode == Keys.Down && Settings.directions != "up")
            {
                goDown = true;
            }
        }

        private void keyIsUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left )
            {
                goLeft = false;
            }
            if (e.KeyCode == Keys.Right )
            {
                goRight = false;
            }
            if (e.KeyCode == Keys.Up )
            {
                goUp = false;
            }
            if (e.KeyCode == Keys.Down )
            {
                goDown = false;
            }
        }

        private void startGame(object sender, EventArgs e)
        {
            restartGame();
        }

        private void takeScreenShot(object sender, EventArgs e)
        {
            //Lable because SaveFileDialog will save only the canvas pic
            Label caption = new Label();
            caption.Text = "I scored: " + score + " and my High score is " + highScore + " on The Snack Game";
            caption.Font = new Font("Times New Roman", 12, FontStyle.Bold);
            caption.ForeColor = Color.Purple;
            caption.AutoSize = false;
            caption.Width = picCanvas.Width;
            caption.Height = 30;
            caption.TextAlign = ContentAlignment.MiddleCenter;
            picCanvas.Controls.Add(caption);

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.FileName = "Snack game screenShot";
            dialog.DefaultExt = "jpg";
            dialog.Filter = "JPG Image File | *.jpg";
            dialog.ValidateNames = true;

            if(dialog.ShowDialog() == DialogResult.OK)
            {
                int width = Convert.ToInt32(picCanvas.Width);
                int height = Convert.ToInt32(picCanvas.Height);
                Bitmap bmp = new Bitmap(width, height);
                picCanvas.DrawToBitmap(bmp, new Rectangle(0, 0, width, height));
                bmp.Save(dialog.FileName , ImageFormat.Jpeg);
                picCanvas.Controls.Remove(caption);
            }
        }

        private void gameTimerEvent(object sender, EventArgs e)
        {

            //######### Setting the directions ############
            if (goLeft)
            {
                Settings.directions = "left";
            }
            if (goRight)
            {
                Settings.directions = "right";
            }
            if (goDown)
            {
                Settings.directions = "down";
            }
            if (goUp)
            {
                Settings.directions = "up";
            }
            //##############################################

            for (int i = snack.Count - 1; i >= 0; i--)
            {
                if (i == 0)
                {

                    switch (Settings.directions)
                    {
                        case "left":
                            snack[i].x--;
                            break;
                        case "right":
                            snack[i].x++;
                            break;
                        case "down":
                            snack[i].y++;
                            break;
                        case "up":
                            snack[i].y--;
                            break;
                    }

                    if (snack[i].x< 0)
                    {
                        snack[i].x = maxWidth;
                    }
                    if (snack[i].x > maxWidth)
                    {
                        snack[i].x = 0;
                    }
                    if (snack[i].y < 0)
                    {
                        snack[i].y = maxHeight;
                    }
                    if (snack[i].y > maxHeight)
                    {
                        snack[i].y = 0;
                    }


                    if (snack[i].x == food.x && snack[i].y == food.y)
                    {
                        eatFood();
                    }

                    for (int j = 1; j < snack.Count; j++)
                    {

                        if (snack[i].x == snack[j].x && snack[i].y == snack[j].y)
                        {
                            gameOver();
                        }

                    }


                }
                else
                {
                    snack[i].x = snack[i - 1].x;
                    snack[i].y = snack[i - 1].y; 
                }
            }


            picCanvas.Invalidate();


        }

        private void updatePicBoxGraphics(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;

            Brush snakeColour;

            for (int i = 0; i < snack.Count; i++)
            {
                if (i == 0)
                {
                    snakeColour = Brushes.Black;
                }
                else
                {
                    snakeColour = Brushes.DarkRed;
                }

                canvas.FillEllipse(snakeColour, new Rectangle(snack[i].x * Settings.width, snack[i].y * Settings.height, Settings.width, Settings.height));
            }


            canvas.FillEllipse(Brushes.DarkGreen, new Rectangle( food.x * Settings.width, food.y * Settings.height,Settings.width, Settings.height));
        }

        private void restartGame()
        {
            maxWidth = picCanvas.Width / Settings.width - 1;
            maxHeight = picCanvas.Height / Settings.height - 1;

            snack.Clear();
            startButton.Enabled = false;
            screenShotBottun.Enabled = false;

            score = 0;
            scoreLabel.Text = "Score: " + score;

            Circle head = new Circle { x = 10 , y = 5};
            snack.Add(head); //here we add a head to be a part of the snack in owr list in index of 0

            //Snack body:
            for(int i = 0; i < 10; i++)
            {
                Circle body = new Circle();
                snack.Add(body);
            }

            food = new Circle { x = rand.Next(2, maxWidth) , y = rand.Next(2, maxHeight)};

            gameTimer.Start();

        }
        
        private void eatFood()
        {
            score += 1;
            scoreLabel.Text = "Score: " + score;
            Circle body = new Circle
            {
                x = snack[snack.Count - 1].x,
                y = snack[snack.Count - 1].y,
            };
            snack.Add(body);

            food = new Circle { x = rand.Next(2, maxWidth), y = rand.Next(2, maxHeight) };
        }

        private void gameOver()
        {
            gameTimer.Stop();
            startButton.Enabled = true;
            screenShotBottun.Enabled = true;
            if(score > highScore)
            {
                highScore = score;
                HighScoreLabel.Text = "High Score: "+ highScore;
                HighScoreLabel.ForeColor = Color.Lime;
            }
        }
    }
}