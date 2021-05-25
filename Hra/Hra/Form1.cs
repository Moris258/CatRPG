using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Media;
using System.Windows.Forms;

namespace Hra
{
    public delegate void ItemFill();

    public partial class Form1 : Form
    {
        public Form1()
        {
            movement.Tile += Player.OnTile;
            movement.Tile += Enemy.OnTile;
            movement.Tile += Level.OnTile;
            
            InitializeComponent();
            //Vytváření různých věcí.
            spelly.Add(new Ability() { name = " ", power = 0, sequence = new int[1] { 0 }, accuracy = 0 });
            spelly.Add(new Ability() { name = "Úder", power = 8, sequence = new int[1] { 0 } , accuracy = 10} );
            spelly.Add(new Ability() { name = "Ohnivá koule", power = 15, sequence = new int[2] { 0, 12 } , accuracy = -5});
            spelly.Add(new Ability() { name = "Facka do ksichtu", power = 6, sequence = new int[3] { 0, 10, 16 } , accuracy = 15});
            spelly.Add(new Ability() { name = "Maturita", power = 13, sequence = new int[4] { 0, 12, 24, 36 } , accuracy = -3});
            spelly.Add(new Ability() { name = "GYMPL", power = 5, sequence = new int[3] { 0, 10, 30 } , accuracy = 20});
            for(int i = 0; i < 4; i++)
            {
                Player.learned.Add(spelly[0]);
            }
            skill1.Text = Player.learned[0].name;

            itemy.Add("HP potion");
            itemy.Add("Bomba");

            enemy.ikona.Add(Properties.Resources.slime);
            enemy.ikona.Add(Properties.Resources.bat);
            enemy.ikona.Add(Properties.Resources.dog);
            enemy.ikona.Add(Properties.Resources.snake);
            enemy.ikona.Add(Properties.Resources.mouse);
            enemy.ikona.Add(Properties.Resources.creepycat);
            enemy.ikona.Add(Properties.Resources.cestina);

            portraits.Add(Properties.Resources.catface1);
            portraits.Add(Properties.Resources.catface2);
            portraits.Add(Properties.Resources.catface3);
            portraits.Add(Properties.Resources.catface4);
            portraits.Add(Properties.Resources.catface5);

            levelicons.Add(Properties.Resources.kocka);
            levelicons.Add(Properties.Resources.kocka2);
            levelicons.Add(Properties.Resources.kocka3);
            levelicons.Add(Properties.Resources.kocka4);
            levelicons.Add(Properties.Resources.kocka5);

        }
        //Globální proměnné
        Player player = new Player(2, 2, 75, 10, 5, "pavel");
        Enemy enemy = new Enemy(5, 5, 10, 2, 2);
        Pohyb movement = new Pohyb();
        Level level = new Level(26, 35);
        int sequence = 0;
        int inuse = 0;
        int skillprob = 1;
        double minigamespeed = 0;
        double progress = 0;
        bool handled = false;
        bool run2 = false;
        double accuracy = 0;
        int points = 30;
        string temporarystate = "";
        

        //Různé listy.
        List<Ability> spelly = new List<Ability>();
        List<string> itemy = new List<string>();
        List<Image> portraits = new List<Image>();
        List<Image> levelicons = new List<Image>();



        //Generace náhodné mapy.
        public int[,] GenerateLevel(Level level)
        {
            
            Random random = new Random();

            for (int x = 0; x < level.grid.GetLength(0); x++)
            {
                for (int y = 0; y < level.grid.GetLength(1); y++)
                {
                    level.grid[x, y] = 0;
                }
            }

            for (int x = 0; x < level.epicentra; x++)
            {
                level.grid[random.Next(5, level.grid.GetLength(0) - 5), random.Next(5,level.grid.GetLength(1) - 5)] = 1;
            }

            return level.grid;
        }


        //Zpřístupnění všech hracích polí. Musí se to dělat ze 3 směrů, aby to vždy fungovalo. Možno změnit pořadí pro jiný vizuální efekt.
        public int[,] LevelFix(Level level)
        {
            Random random = new Random();
            int direction = 0;

            for (int x = 0; x < level.grid.GetLength(0); x++)
            {
                for (int y = 0; y < level.grid.GetLength(1); y++)
                {
                    if (level.grid[x, y] == 1)
                    {
                        if (x == level.grid.GetLength(0) - 1 || y == level.grid.GetLength(1) - 1)
                        {

                        }

                        else if (level.grid[x, y + 1] == 0 && level.grid[x + 1, y] == 0)
                        {
                            direction = random.Next(1, 3);

                            if (direction == 1)
                            {
                                level.grid[x + 1, y] = 1;
                            }
                            else if (direction == 2)
                            {
                                level.grid[x, y + 1] = 1;
                            }
                        }
                    }
                }
            }


            for (int x = level.grid.GetLength(0) - 1; x >= 0; x--)
            {
                for (int y = level.grid.GetLength(1) - 1; y >= 0; y--)
                {
                    if (level.grid[x, y] == 1)
                    {
                        if (x == 0 || y == 0)
                        {

                        }

                        else if (level.grid[x, y - 1] == 0 && level.grid[x - 1, y] == 0)
                        {
                            direction = random.Next(1, 3);

                            if (direction == 1)
                            {
                                level.grid[x - 1, y] = 1;
                            }
                            else if (direction == 2)
                            {
                                level.grid[x, y - 1] = 1;
                            }
                        }
                    }
                }
            }

            for (int x = level.grid.GetLength(0) - 1; x >= 0; x--)
            {
                for (int y = 0; y < level.grid.GetLength(1) - 1; y++)
                {
                    if (level.grid[x, y] == 1)
                    {
                        if (x == 0 || y == level.grid.GetLength(1) - 1)
                        {

                        }

                        else if (level.grid[x, y + 1] == 0 && level.grid[x - 1, y] == 0)
                        {
                            direction = random.Next(1, 3);

                            if (direction == 1)
                            {
                                level.grid[x - 1, y] = 1;
                            }
                            else if (direction == 2)
                            {
                                level.grid[x, y + 1] = 1;
                            }
                        }
                    }
                }
            }

            return level.grid;

        }

        //Přídá do mapy stěny, aby to hezky vypadalo.
        public int[,] WallSpawn(Level level)
        {
            for (int x = 0; x < level.grid.GetLength(0); x++)
            {
                for (int y = 0; y < level.grid.GetLength(1); y++)
                {
                    if (x == level.grid.GetLength(0) - 1 || y == level.grid.GetLength(1) - 1 || x == 0 || y == 0)
                    {

                    }

                    else if(level.grid[x,y] != 0 && level.grid[x, y] != 2)
                    {
                        if (level.grid[x + 1, y] == 0)
                            level.grid[x + 1, y] = 2;

                        if (level.grid[x - 1, y] == 0)
                            level.grid[x - 1, y] = 2;

                        if (level.grid[x - 1, y + 1] == 0)
                            level.grid[x - 1, y + 1] = 2;

                        if (level.grid[x + 1, y + 1] == 0)
                            level.grid[x + 1, y + 1] = 2;

                        if (level.grid[x - 1,y - 1] == 0)
                            level.grid[x - 1, y - 1] = 2;

                        if (level.grid[x + 1, y - 1] == 0)
                            level.grid[x + 1, y - 1] = 2;

                        if (level.grid[x, y + 1] == 0)
                            level.grid[x, y + 1] = 2;

                        if (level.grid[x, y - 1] == 0)
                            level.grid[x, y - 1] = 2;

                    }
                }
            }


            return level.grid;
        }


        //Vygeneruje předměty a postavy na mapě
        public int[,] EntitySpawn(Level level, Player player)
        {
            Random random = new Random();
            bool valid = true;
            int x;
            int y;

            while (valid)
            {
                x = random.Next(0, level.grid.GetLength(0));
                y = random.Next(0, level.grid.GetLength(1));
                if(level.grid[x,y] == 1)
                {
                    level.grid[x, y] = 5;
                    valid = false;
                }
            }

            valid = true;

            while (valid)
            {
                player.posx = random.Next(0, level.grid.GetLength(0));
                player.posy = random.Next(0, level.grid.GetLength(1));
                if (level.grid[player.posx, player.posy] == 1)
                {
                    level.grid[player.posx, player.posy] = 3;
                    valid = false;
                }
            }

            for (int i = 0; i < level.pocet_nepratel; i++)
            {
                valid = true;
                while (valid)
                {
                    x = random.Next(0, level.grid.GetLength(0));
                    y = random.Next(0, level.grid.GetLength(1));
                    if (level.grid[x, y] == 1)
                    {
                        level.grid[x, y] = 4;
                        valid = false;
                    }
                }
            }
            
            for (int i = 0; i < level.pocet_truhel; i++)
            {
                valid = true;
                while (valid)
                {
                    x = random.Next(0, level.grid.GetLength(0));
                    y = random.Next(0, level.grid.GetLength(1));
                    if (level.grid[x, y] == 1)
                    {
                        level.grid[x, y] = 6;
                        valid = false;
                    }
                }
            }


            return level.grid;
        }

        /*První vykreslení mapy.
         1 - zem
         2 - zed
         3 - hrac
         4 - nepřítel
         5 - exit
         6 - truhla
         7 - otevreny exit
         0 - temnota */
        public void DrawRoom(Level level)
        {
            Graphics g = pictureBox1.CreateGraphics();
            Image newImage;
            Point point;

            for(int x = 0; x < level.grid.GetLength(0); x++)
            {
                for(int y = 0; y < level.grid.GetLength(1); y++)
                {
                    switch (level.grid[x, y])
                    {
                        case 1:
                            newImage = Properties.Resources.zem;
                            point = new Point(x * 32, y * 32);

                            g.DrawImage(newImage, point);
                            break;
                        case 2:
                            newImage = Properties.Resources.zed;
                            point = new Point(x * 32, y * 32);

                            g.DrawImage(newImage, point);
                            break;
                        case 3:
                            newImage = player.gridicon;
                            point = new Point(x * 32, y * 32);

                            g.DrawImage(newImage, point);
                            break;
                        case 4:
                            newImage = Properties.Resources.enemy;
                            point = new Point(x * 32, y * 32);

                            g.DrawImage(newImage, point);
                            break;
                        case 5:
                            newImage = Properties.Resources.dvere;
                            point = new Point(x * 32, y * 32);

                            g.DrawImage(newImage, point);
                            break;
                        case 6:
                            newImage = Properties.Resources.truhla;
                            point = new Point(x * 32, y * 32);

                            g.DrawImage(newImage, point);
                            break;
                        case 7:
                            newImage = Properties.Resources.otevrene_dvere;
                            point = new Point(x * 32, y * 32);

                            g.DrawImage(newImage, point);
                            break;
                        default:
                            newImage = Properties.Resources.guma;
                            point = new Point(x * 32, y * 32);

                            g.DrawImage(newImage, point);
                            break;





                    }
                }
            }
            
        }

        //Updatování specifického bodu na mapě.
        public void ChangeTile(int x, int y)
        {

            Graphics g = pictureBox1.CreateGraphics();
            Image newImage = Properties.Resources.zem;
            Point point = new Point(x * 32, y * 32);

            g.DrawImage(newImage, point);



        }
        
        //Generace nového levelu
        public Level ResetLevel(Level level)
        {
            Random random = new Random();
            level.epicentra = random.Next(3, 7);
            level.pocet_nepratel = random.Next(4, level.epicentra * 2 + 3);
            level.pocet_truhel = random.Next(2, level.epicentra + 1);


            return level;
        }

        //Event o pohybu na mapě.
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            ItemFill filldelegate = new ItemFill(InventoryFill);
            Random random = new Random();
            if (player.state == "pohyb")
            {
                try
                {
                    Pohyb.Movement(level, player, e.KeyValue, pictureBox1, movement, pictureBox2, enemy, label3, panel2, backgroundWorker1, itemy, richTextBox2, label6,  panel6, panel5, filldelegate);

                }
                catch
                {

                }
            }
            else if (player.state == "enemy" && backgroundWorker2.IsBusy || player.state == "boss" && backgroundWorker2.IsBusy)
            {
                if (e.KeyValue == 87)
                {
                    if (sequence < Player.learned[inuse].sequence.GetLength(0) - 1)
                    {
                        sequence++;
                        accuracy = accuracy + progress;
                    }
                    else
                    {
                        accuracy = accuracy + progress;
                        accuracy = accuracy / Player.learned[inuse].sequence.GetLength(0);
                        backgroundWorker2.CancelAsync();
                        double var = random.Next(-5, 6);
                        SoundPlayer sound;
                        int damage = Convert.ToInt32(Math.Round((player.atk + Player.learned[inuse].power + var) * Math.Pow((accuracy * 2 / 100), level.gamedifficulty)) - enemy.def);

                        if (accuracy + Player.learned[inuse].accuracy < enemy.dodge + random.Next(0, 6))
                        {
                            if (accuracy >= 48.5)
                            {
                                enemy.hp -= damage;
                                richTextBox1.AppendText(String.Format("Udělil jsi {0} damage." + Environment.NewLine, damage));
                                sound = new SoundPlayer(Properties.Resources.attack);
                                sound.PlaySync();
                            }
                            else
                            {
                                richTextBox1.AppendText(String.Format("Nepřítel se vyhnul." + Environment.NewLine));
                                sound = new SoundPlayer(Properties.Resources.miss);
                                sound.PlaySync();
                            }
                        }
                        else if(damage <= 0)
                        {
                            richTextBox1.AppendText(String.Format("Nedal jsi žádný damage." + Environment.NewLine));
                            sound = new SoundPlayer(Properties.Resources.armorchink);
                            sound.PlaySync();
                        }
                        else if(random.Next(0,10) == 0)
                        {
                            damage = damage * 2;
                            enemy.hp -= damage;
                            richTextBox1.AppendText(String.Format("Kritický zásah! Udělil jsi {0} damage." + Environment.NewLine, damage));
                            sound = new SoundPlayer(Properties.Resources.criticalhit);
                            sound.PlaySync();
                        }
                        else if (0 < damage)
                        {
                            enemy.hp -= damage;
                            richTextBox1.AppendText(String.Format("Udělil jsi {0} damage." + Environment.NewLine, damage));
                            sound = new SoundPlayer(Properties.Resources.attack);
                            sound.PlaySync();
                        }
                        
                        label3.Text = String.Format("HP: {0}", enemy.hp);
                        richTextBox1.SelectionStart = richTextBox1.Text.Length;
                        richTextBox1.ScrollToCaret();
                        label4.Text = String.Format("HP: {0}/{1}", player.hp,player.maxhp);
                        accuracy = 0;

                    }



                }
            }
        }
        
        //Zobrazuje vytváření nové postavy
        private void button1_Click(object sender, EventArgs e)
        {
            panel1.Hide();
            panel7.Visible = true;
            if(run2 == false)
            {
                for (int i = 2; i < spelly.Count; i++)
                {
                    startability.Items.Add(spelly[i].name);
                }
                for (int i = 0; i < portraits.Count; i++)
                {
                    playerportrait.Items.Add(String.Format("Pozadí {0}", i + 1));
                }
                for (int i = 0; i < levelicons.Count; i++)
                {
                    playericon.Items.Add(String.Format("Ikona hráče {0}", i + 1));
                }
            }
            else
            {
                for(int i = startability.Items.Count + 2; i < spelly.Count; i++)
                {
                    startability.Items.Add(spelly[i].name);
                }
            }
            body.Text = String.Format("Zbývající body: {0}", points);
        }

        //Dělá různé pomocné věci
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Random random = new Random();
            
            if(player.state == "pohyb" && level.repaint == true)
            {
                level.repaint = true;
                Thread.Sleep(100);
                DrawRoom(level);
                level.repaint = false;

            }
            else if(player.state == "enemy" || player.state == "boss")
            {
                Image newImage;
                if (player.state == "enemy")
                {
                    newImage = enemy.ikona[random.Next(0, 5)];
                }
                else
                {
                    newImage = enemy.ikona[random.Next(5, 7)];
                }
                enemypanel.BackgroundImage = newImage;
            }
            else if (player.state == "help")
            {
                Thread.Sleep(100);
                DrawRoom(level);
                Graphics g = pictureBox1.CreateGraphics();
                Image newImage = player.gridicon;
                Point point = new Point(level.exitposx * 32, level.exitposy * 32);

                g.DrawImage(newImage, point);
            }
        }

        //Zobrazuje skilly
        private void button5_Click(object sender, EventArgs e)
        {
            panel2.Hide();
            panel4.Show();
        }

        //Dělá minihru
        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            double pocet = minigamespeed + Player.learned[inuse].sequence[Player.learned[inuse].sequence.GetLength(0) - 1];
            
            for (int i = 1; i <= pocet; i++)
            {
                Thread.Sleep(16);
                Image newImage;
                Point point;
                Graphics g = pictureBox2.CreateGraphics();
                newImage = Properties.Resources.AttackArea;
                point = new Point(17, 0);
                g.DrawImage(newImage, point);
                for (int k = 0; k < Player.learned[inuse].sequence.GetLength(0); k++) 
                {
                    if (Player.learned[inuse].sequence[k] < i && sequence <= k )
                    {
                        newImage = Properties.Resources.Line;
                        point = new Point(Convert.ToInt32(17 + (i - Player.learned[inuse].sequence[k]) * (400 / minigamespeed) ), 0);
                        g.DrawImage(newImage, point);
                    }

                }
                
                if (minigamespeed / 2 + Player.learned[inuse].sequence[sequence] <= i)
                {
                    backgroundWorker2.ReportProgress(Convert.ToInt32(minigamespeed - ((i - Player.learned[inuse].sequence[sequence]) - minigamespeed / 2) * 2));
                }
                else
                {
                    backgroundWorker2.ReportProgress((i - Player.learned[inuse].sequence[sequence]) * 2);
                }
                if (backgroundWorker2.CancellationPending)
                {
                    //Thread.Sleep(100);
                    break;
                }

            }
        }

        //Reportuje progress
        private void backgroundWorker2_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progress = e.ProgressPercentage * 50/minigamespeed;
        }

        //Resolvuje smrt hráče, nepřítele nebo konec kola.
        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            accuracy = 0;
            if (0 < enemy.hp)
            {
                player.hp = EnemyCombat(player, enemy, richTextBox1);
                label4.Text = String.Format("HP: {0}/{1}", player.hp, player.maxhp);
                panel2.Show();
            }
            EntityDeath();

        }

        //Řeší smrt hráče nebo nepřítele a level up.
        public void EntityDeath()
        {
            Random random = new Random();
            double exp = 0;
            SoundPlayer sound;
            if (player.hp <= 0)
            {
                GameOver();


            }
            else if (enemy.hp <= 0)
            {


                if (player.state == "boss")
                {
                    if(level.scale < player.level)
                        exp = Math.Round((double)random.Next(35, 50) / player.level + 1 - level.scale);
                    else
                        exp = Math.Round((double)random.Next(35, 50));

                }
                else
                {
                    if (level.scale < player.level)
                        exp = Math.Round((double)random.Next(15, 25) / player.level + 1 - level.scale);
                    else
                        exp = Math.Round((double)random.Next(15, 25));
                }

                richTextBox1.AppendText(String.Format("Porazil jsi nepřítele." + Environment.NewLine));
                richTextBox1.AppendText(String.Format("Dostal jsi {0} EXP." + Environment.NewLine, exp));

                player.exp += exp;
                if (99 < player.exp)
                {
                    sound = new SoundPlayer(Properties.Resources.chipquest);
                    sound.PlaySync();
                    player.level += 1;
                    player.exp -= 100;
                    player.maxhp += 10;
                    player.hp = player.maxhp;
                    player.atk += 3;
                    player.def += 3;

                    label4.Text = String.Format("HP: {0}/{1}", player.hp, player.maxhp);
                    atklabel.Text = String.Format("Útok: {0}", player.atk);
                    deflabel.Text = String.Format("Obrana: {0}", player.def);

                    richTextBox1.AppendText(String.Format("Level Up." + Environment.NewLine, exp));
                    label5.Text = String.Format("Level: {0}", player.level);
                    if (random.Next(skillprob, 4) == 3)
                    {
                        temporarystate = "new ability";
                        skillprob = 1;
                    }
                    else
                    {
                        skillprob += 1;
                    }


                }
                label8.Text = String.Format("EXP: {0}", player.exp);
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();

                if (temporarystate == "new ability")
                {
                    novaschopnost.Show();
                }
                else
                {
                    CombatEnd();
                }



            }
            else
            {
                panel2.Show();
            }
        }
        
        //Exit Game button
        private void button4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        //Funkce pro řešení kombatu pro nepřítele.
        public int EnemyCombat(Player player, Enemy enemy, RichTextBox richTextBox1)
        {
            Random random = new Random();
            int damage = enemy.attack + random.Next(-5, 6) - player.def;
            SoundPlayer sound;
            if(sequence >= Player.learned[inuse].sequence.GetLength(0) - 1)
            {
                if (random.Next(0, 10) == 0)
                {
                    richTextBox1.AppendText(String.Format("Nepřítel se netrefil." + Environment.NewLine));
                    sound = new SoundPlayer(Properties.Resources.miss);
                    sound.PlaySync();
                }
                else if(damage <= 0)
                {
                    richTextBox1.AppendText(String.Format("Nepřítel ti nic neudělal" + Environment.NewLine));
                    sound = new SoundPlayer(Properties.Resources.armorchink);
                    sound.PlaySync();
                }
                else if (random.Next(0, 10) == 0)
                {
                    damage = damage * 2;
                    richTextBox1.AppendText(String.Format("Kritický zásah! Nepřítel ti udělil {0} damage" + Environment.NewLine, damage));
                    player.hp -= damage;
                    sound = new SoundPlayer(Properties.Resources.criticalhit);
                    sound.PlaySync();
                }
                else if (0 < damage)
                {
                    richTextBox1.AppendText(String.Format("Nepřítel ti udělil {0} damage" + Environment.NewLine, damage));
                    player.hp -= damage;
                    sound = new SoundPlayer(Properties.Resources.attack);
                    sound.PlaySync();
                }
            }
            
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();

            return player.hp;
        }

        //Aby spacebar nefungoval.
        private void button5_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
                handled = true;
        }

        //Manažuje věci na konci combatu.
        public void CombatEnd()
        {
            if (player.state == "boss")
            {
                pictureBox2.Image = null;
                enemypanel.BackgroundImage = null;
                panel2.Hide();
                label3.Hide();

                panel6.Show();
            }
            else
            {
                player.state = "pohyb";
                pictureBox2.Image = null;
                enemypanel.BackgroundImage = null;
                panel2.Hide();
                label3.Hide();
                level.grid[enemy.posx, enemy.posy] = 1;
                ChangeTile(enemy.posx, enemy.posy);
            }
        }

        //Manažuje věci při game over.
        public void GameOver()
        {
            SoundPlayer sound;
            sound = new SoundPlayer(Properties.Resources.gameoversound);
            sound.PlaySync();
            player.state = "dead";
            for(int i = 0; i < player.inventory.Count; i++)
            {
                player.inventory[i] = "";
            }
            for(int i = 0; i < 4; i++)
            {
                Player.learned[i] = spelly[0];
            }
            
            enemypanel.BackgroundImage = null;
            pictureBox1.Hide();
            pictureBox1.Image = null;
            pictureBox2.Hide();
            panel2.Hide();
            panel3.Hide();
            panel4.Hide();
            panel5.Hide();
            enemypanel.Hide();
            backtomenupanel.Hide();
            novaschopnost.Hide();
            slotpicklabel.Hide();
            label6.Hide();
            richTextBox1.Hide();
            richTextBox1.Clear();
            richTextBox2.Hide();
            richTextBox2.Clear();
            label3.Hide();
            urovenlabel.Hide();
            button22.Visible = false;
            level.scale = 1;
            this.BackgroundImage = Properties.Resources.gameoverscreen;
            gameoverpanel.Location = new Point(500, 300);
            gameoverpanel.Show();
            points = (int)pointsoption.Value;
            run2 = true;
            zivotyvalue.Text = "75";
            utokvalue.Text = "10";
            obranavalue.Text = "5";
            player.maxhp = 75;
            player.atk = 10;
            player.def = 5;
            player.level = 1;
            player.exp = 0;
            level.repaint = true;

        }

        //Používání skillu nebo výběr slotu
        private void skill_Click(object sender, EventArgs e)
        {
            Random random = new Random();
            Button button = (Button)sender;
            if (handled)
            {
                handled = false;
                return;
            }
            
            if(temporarystate == "new ability")
            {
                button.Text = spelly[spelly.Count - 1].name;
                Player.learned[Convert.ToInt32(button.AccessibleDescription)] = spelly[spelly.Count - 1];
                playername.Focus();
                CombatEnd();
                slotpicklabel.Hide();
                panel4.Hide();
                temporarystate = "";
            }
            else
            {
                if (Player.learned[Convert.ToInt32(button.AccessibleDescription)].name == " ")
                {

                }
                else
                {
                    try
                    {
                        sequence = 0;
                        minigamespeed = random.Next(level.gamedifficulty * 18 + 20 , level.gamedifficulty * 18 + 36);
                        panel4.Hide();
                        Graphics g = pictureBox2.CreateGraphics();
                        Image newImage = Properties.Resources.AttackArea;
                        Point point = new Point(17, 550);
                        g.DrawImage(newImage, point);
                        newImage = Properties.Resources.Line;
                        point = new Point(17, 550);
                        g.DrawImage(newImage, point);
                        backgroundWorker2.RunWorkerAsync();
                        inuse = Convert.ToInt32(button.AccessibleDescription);
                    }
                    catch
                    {

                    }
                }
            }

            
            
        }

        //Backbutton u skillů
        private void backbttn_Click(object sender, EventArgs e)
        {
            if(temporarystate != "new ability")
            {
                panel4.Hide();
                panel2.Show();
            }
        }
        
        //Zobrazení okna itemů
        private void button6_Click(object sender, EventArgs e)
        {
            panel5.Show();
            panel2.Hide();
            richTextBox2.Hide();
            InventoryFill();
        }
        
        //Naplní labely itemů při kombatu nebo invfull.
        public void InventoryFill()
        {
            for (int i = 0; i < player.inventory.Count; i++)
            {
                switch (i + 1)
                {
                    case 1:
                        item1.Text = player.inventory[i];
                        break;
                    case 2:
                        item2.Text = player.inventory[i];
                        break;
                    case 3:
                        item3.Text = player.inventory[i];
                        break;
                    case 4:
                        item4.Text = player.inventory[i];
                        break;
                    case 5:
                        item5.Text = player.inventory[i];
                        break;
                    case 6:
                        item6.Text = player.inventory[i];
                        break;
                    case 7:
                        item7.Text = player.inventory[i];
                        break;
                    case 8:
                        item8.Text = player.inventory[i];
                        break;
                    case 9:
                        item9.Text = player.inventory[i];
                        break;
                    case 10:
                        item10.Text = player.inventory[i];
                        break;

                }
            }
            
        }

        //Item tooltip show
        private void item_MouseEnter(object sender, EventArgs e)
        {
            Label item = (Label)sender;
            item.BackColor = Color.DimGray;
        }

        //Item tooltip hide
        private void item_MouseLeave(object sender, EventArgs e)
        {
            Label item = (Label)sender;
            item.BackColor = Color.Transparent;
            itemtooltippanel.Hide();
        }
        
        //Použití nebo nahrazení itemů.
        private void item_Click(object sender, EventArgs e)
        {
            Label item = (Label)sender;
            bool finished = false;
            SoundPlayer sound;
            if(itemy.Contains(player.state))
            {
                player.inventory[Convert.ToInt32(item.AccessibleDescription) - 1] = player.state;
                player.state = "pohyb";
                label6.Hide();
                richTextBox2.Clear();
                for (int i = 0; i < player.inventory.Count; i++)
                {
                    richTextBox2.AppendText(String.Format("{1}: {0}" + Environment.NewLine, player.inventory[i], i + 1));
                }
                panel5.Hide();

            }
            else
            {
                if (Convert.ToInt32(item.AccessibleDescription) <= player.inventory.Count)
                {
                    if (player.inventory[Convert.ToInt32(item.AccessibleDescription) - 1] != "")
                    {
                        switch (player.inventory[Convert.ToInt32(item.AccessibleDescription) - 1])
                        {
                            case "HP potion":
                                if (player.hp <= player.maxhp && player.maxhp - 35 < player.hp)
                                {
                                    player.hp = player.maxhp;
                                }
                                else
                                {
                                    player.hp += 35;
                                }
                                richTextBox1.AppendText(String.Format("Doplněno 35hp." + Environment.NewLine));
                                sound = new SoundPlayer(Properties.Resources.potion);
                                sound.PlaySync();
                                break;
                            case "Bomba":
                                enemy.hp -= 30;
                                label3.Text = String.Format("HP: {0}", enemy.hp);
                                richTextBox1.AppendText(String.Format("Bomba udělila 30 damage." + Environment.NewLine));
                                sound = new SoundPlayer(Properties.Resources.bomba);
                                sound.PlaySync();
                                EntityDeath();
                                break;

                        }
                        richTextBox1.SelectionStart = richTextBox1.Text.Length;
                        richTextBox1.ScrollToCaret();
                        player.inventory[Convert.ToInt32(item.AccessibleDescription) - 1] = "";
                        finished = true;
                    }
                }
                if (finished)
                {
                    richTextBox2.Clear();
                    for (int i = 0; i < player.inventory.Count; i++)
                    {
                        richTextBox2.AppendText(String.Format("{1}: {0}" + Environment.NewLine, player.inventory[i], i + 1));
                    }
                    panel5.Hide();
                    if(0 < enemy.hp)
                    {
                        sequence = Player.learned[inuse].sequence.GetLength(0) - 1;
                        player.hp = EnemyCombat(player, enemy, richTextBox1);
                        label4.Text = String.Format("HP: {0}/{1}", player.hp, player.maxhp);
                        panel2.Show();
                    }
                    richTextBox2.Show();
                }
            }
            


        }
        
        //Backbutton u itemů
        private void bckbutton2_Click(object sender, EventArgs e)
        {
            if (itemy.Contains(player.state))
            {
                player.state = "pohyb";
                label6.Hide();
                panel5.Hide();


            }
            else
            {
                panel5.Hide();
                panel2.Show();
                richTextBox2.Show();
            }
            
        }

        //Vstup do nového levelu
        private void yesbbtn_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            panel6.Hide();
            level = ResetLevel(level);
            level.grid = GenerateLevel(level);
            level.grid = LevelFix(level);
            level.grid = WallSpawn(level);
            level.grid = EntitySpawn(level, player);
            level.scale += 1;
            urovenlabel.Text = String.Format("Úroveň: {0}", level.scale);
            player.state = "pohyb";
            backgroundWorker1.RunWorkerAsync();
        }

        //NO button u exitu do nového levlu
        private void nobttn_Click(object sender, EventArgs e)
        {
            panel6.Hide();
            player.state = "help";
            level.grid[level.exitposx, level.exitposy] = 7;
            ChangeTile(player.posx, player.posy);
            level.grid[player.posx, player.posy] = 1;

            backgroundWorker1.RunWorkerAsync();
            
            player.posx = level.exitposx;
            player.posy = level.exitposy;

        }

        //Blbost pro exit
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(player.state == "help")
            {
                player.state = "pohyb";
            }
        }

        //Start hry.
        private void button15_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            if(playername.Text == "")
            {
                errormsg.Text = "Vyberte si prosím jméno.";
            }
            else if (!startability.Items.Contains(startability.Text))
            {
                errormsg.Text = "Vyberte si prosím začáteční schopnost.";
            }
            else if (!playericon.Items.Contains(playericon.Text))
            {
                errormsg.Text = "Vyberte si prosím hráčskou ikonu.";
            }
            else if (!playerportrait.Items.Contains(playerportrait.Text))
            {
                errormsg.Text = "Vyberte si prosím portrét.";
            }
            else
            {
                if(0 < points && button.AccessibleDescription != "1" )
                {
                    alertpanel.Visible = true;
                }
                else
                {
                    level.repaint = true;
                    player.state = "pohyb";
                    player.hp = player.maxhp;
                    player.maxhp = Convert.ToInt32(zivotyvalue.Text);
                    player.atk = Convert.ToInt32(utokvalue.Text);
                    player.def = Convert.ToInt32(obranavalue.Text);
                    player.name = playername.Text;
                    Player.learned[0] = spelly[1];
                    skill1.Text = Player.learned[0].name;
                    Player.learned[1] = spelly[startability.Items.IndexOf(startability.Text) + 2];
                    skill2.Text = Player.learned[1].name;
                    player.potrait = playerportraitpanel.BackgroundImage;
                    player.gridicon = new Bitmap(playericonpanel.BackgroundImage, new Size(32, 32));

                    level.gamedifficulty = difficultyoption.Items.IndexOf(difficultyoption.Text) + 1;
                    level.avaragesize = levelsizeoption.Items.IndexOf(levelsizeoption.Text) + 1;

                    playerportraitgame.BackgroundImage = player.potrait;

                    displayname.Text = player.name;
                    atklabel.Text = String.Format("Útok: {0}", player.atk);
                    deflabel.Text = String.Format("Obrana: {0}", player.def);
                    errormsg.Text = "";
                    player.name = playername.Text;

                    panel7.Hide();
                    panel2.Visible = true;
                    panel2.Hide();
                    alertpanel.Hide();
                    label3.Hide();
                    label6.Visible = true;
                    label6.Hide();
                    slotpicklabel.Visible = true;
                    slotpicklabel.Hide();
                    gameoverpanel.Visible = true;
                    gameoverpanel.Hide();
                    itemtooltippanel.Visible = true;
                    itemtooltippanel.Hide();
                    urovenlabel.Visible = true;
                    urovenlabel.Text = String.Format("Úroveň: {0}", level.scale);

                    button22.Visible = true;
                    
                    richTextBox1.Visible = true;
                    richTextBox2.Visible = true;

                    panel3.Visible = true;
                    panel4.Visible = true;
                    panel4.Hide();
                    panel6.Visible = true;
                    panel6.Hide();
                    enemypanel.Visible = true;
                    novaschopnost.Visible = true;
                    novaschopnost.Hide();
                    skillhoverpanel.Visible = true;
                    skillhoverpanel.Hide();
                    backtomenupanel.Visible = true;
                    backtomenupanel.Hide();

                    label4.Text = String.Format("HP: {0}/{1}", player.hp, player.maxhp);
                    label5.Text = String.Format("Level: {0}", player.level);
                    label8.Text = String.Format("EXP: {0}", player.exp);

                    level = ResetLevel(level);
                    level.grid = GenerateLevel(level);
                    level.grid = LevelFix(level);
                    level.grid = WallSpawn(level);
                    level.grid = EntitySpawn(level, player);

                    pictureBox1.Visible = true;
                    pictureBox2.Visible = true;

                    backgroundWorker1.RunWorkerAsync();
                    this.BackgroundImage = null;
                    panel5.Show();
                    panel5.Hide();
                }
                
            }
            
        }
        
        //Změna atribut u vytváření nové postavy
        private void attributechange_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;

            switch (button.AccessibleDescription)
            {
                case "zivoty-":
                    if(75 < player.maxhp)
                    {
                        player.maxhp -= 5;
                        points++;
                    }
                    break;
                case "zivoty+":
                    if(0 < points)
                    {
                        player.maxhp += 5;
                        points--;
                    }
                    break;
                case "utok-":
                    if (10 < player.atk)
                    {
                        player.atk -= 1;
                        points++;
                    }

                    break;
                case "utok+":
                    if (0 < points)
                    {
                        player.atk += 1;
                        points--;
                    }
                    break;
                case "obrana-":
                    if (10 < player.def)
                    {
                        player.def -= 1;
                        points++;
                    }
                    break;
                case "obrana+":
                    if (0 < points)
                    {
                        player.def += 1;
                        points--;
                    }
                    break;

            }
            body.Text = String.Format("Zbývající body: {0}", points);
            zivotyvalue.Text = Convert.ToString(player.maxhp);
            utokvalue.Text = Convert.ToString(player.atk);
            obranavalue.Text = Convert.ToString(player.def);
        }

        //repaint pictureboxu
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (level.repaint == false)
            {
                backgroundWorker3.CancelAsync();
                Thread.Sleep(10);
                if(backgroundWorker3.IsBusy == false)
                {
                    backgroundWorker3.RunWorkerAsync();
                }
            }
        }

        //Změna obrázku hráčského portrétu
        private void playerportrait_TextChanged(object sender, EventArgs e)
        {
            playerportraitpanel.BackgroundImage = portraits[playerportrait.Items.IndexOf(playerportrait.Text)];
        }

        //Změna obrázku hráčské ikony
        private void playericon_TextChanged(object sender, EventArgs e)
        {
            playericonpanel.BackgroundImage = new Bitmap(levelicons[playericon.Items.IndexOf(playericon.Text)],playericonpanel.Size);
        }

        //Odpověď NE na zbývající body.
        private void button16_Click(object sender, EventArgs e)
        {
            alertpanel.Visible = false;
        }

        //Generace náhodných skillů.
        private void button17_Click(object sender, EventArgs e)
        {
            Random random = new Random();
            bool contains = false;
            if(abilityname.Text != "")
            {
                for(int i = 0; i < spelly.Count; i++)
                {
                    if(spelly[i].name == abilityname.Text)
                    {
                        contains = true;
                    }
                }
                if(contains != true)
                {
                    int[] sequence = new int[random.Next(1, 8)];
                    int power = random.Next(0, 15);
                    for (int i = 1; i < sequence.GetLength(0); i++)
                    {
                        sequence[i] = sequence[i - 1] + random.Next(6, 21);
                    }

                    spelly.Add(new Ability() { name = abilityname.Text, power = power + sequence.GetLength(0) + player.level * 2, sequence = sequence, accuracy = random.Next(-power + 5 + player.level * 2, 20 - power + player.level * 2) });
                    panel4.Show();
                    novaschopnost.Hide();
                    slotpicklabel.Show();
                    label14.Text = "Naučil jste se novou schopnost, dejte ji prosím jméno.";
                }
                else
                {
                    label14.Text = "Schopnost s tímto jménem již existuje.";
                }
                
            }
            
        }

        //Tooltip show
        private void skill_MouseHover(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            if(button.Text != "")
            {
                skillhoverpanel.Location = this.PointToClient(Cursor.Position);
                skillhoverpanel.Location = new Point(skillhoverpanel.Location.X + 10, skillhoverpanel.Location.Y + 10);
                skillhoverpanel.Show();
                damagehoverlabel.Text = String.Format("DMG: {0}", Player.learned[Convert.ToInt32(button.AccessibleDescription)].power);
                accuracyhoverlabel.Text = String.Format("ACC: {0}", Player.learned[Convert.ToInt32(button.AccessibleDescription)].accuracy);
            }
            

        }

        //Skrytí tooltipu skillu
        private void skill_MouseLeave(object sender, EventArgs e)
        {
            skillhoverpanel.Hide();
        }

        //Změna textu při výběru startovního skillu
        private void startability_TextChanged(object sender, EventArgs e)
        {
            startspelldmg.Text = String.Format("DMG: {0}", spelly[startability.Items.IndexOf(startability.Text) + 2].power);
            startspellacc.Text = String.Format("ACC: {0}", spelly[startability.Items.IndexOf(startability.Text) + 2].accuracy);
        }

        //Options panel
        private void button2_Click(object sender, EventArgs e)
        {
            optionspanel.Visible = true;
        }

        //Options panel back button.
        private void button18_Click(object sender, EventArgs e)
        {
            optionspanel.Visible = false;
            points = (int)pointsoption.Value;
        }

        //Vytváření nové postavy backbutton.
        private void button19_Click(object sender, EventArgs e)
        {
            panel7.Hide();
            panel1.Show();
        }

        //Zrušení učení nového skillu.
        private void cancelbutton_Click(object sender, EventArgs e)
        {
            novaschopnost.Hide();
            temporarystate = "";
            label14.Text = "Naučil jste se novou schopnost, dejte ji prosím jméno.";
            playername.Focus();
            CombatEnd();
        }

        //Back to menu po Game Over
        private void button20_Click(object sender, EventArgs e)
        {
            gameoverpanel.Hide();
            this.BackgroundImage = Properties.Resources.pozadi1;
            panel1.Show();
        }

        //Jak hrát?
        private void button3_Click(object sender, EventArgs e)
        {
            Form2 newform = new Form2();
            newform.ShowDialog();
        }

        //Tooltip pro itemy
        private void item_MouseHover(object sender, EventArgs e)
        {
            Label label = (Label)sender;
            itemtooltippanel.Location = this.PointToClient(Cursor.Position);
            itemtooltippanel.Location = new Point(itemtooltippanel.Location.X + 10, itemtooltippanel.Location.Y + 10);
            switch (label.Text)
            {
                case "HP potion":
                    itemtooltiplabel.Text = "Obnoví 35 hp.";
                    itemtooltippanel.Show();
                    break;
                case "Bomba":
                    itemtooltiplabel.Text = "Dá 30 dmg.";
                    itemtooltippanel.Show();
                    break;
                default:
                    break;
            }
            
        }

        //Back to menu button
        private void button22_Click(object sender, EventArgs e)
        {
            if (handled)
            {
                handled = false;
                return;
            }
            backtomenupanel.Show();
        }
        
        //Pro repaint
        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
            if (level.repaint == false)
            {
                Thread.Sleep(300);
                DrawRoom(level);
            }
        }
        
        //Back to menu potvrzení
        private void returnbttn_Click(object sender, EventArgs e)
        {
            if (handled)
            {
                handled = false;
                return;
            }
            GameOver();
        }

        //Odpověď NE na Back to menu
        private void cancelreturnbttn_Click(object sender, EventArgs e)
        {
            backtomenupanel.Hide();
        }

        //Odfocusuje to richtextbox, aby to furt nebimbalo, když na to uživatel klikne.
        private void richTextBox2_Click(object sender, EventArgs e)
        {
            button22.Focus();
        }
    }

    public class Enemy
    {
        public int posx;
        public int posy;
        public int hp;
        public int attack;
        public int def;
        public int dodge;
        public List<Image> ikona = new List<Image>();

        public Enemy(int posx, int posy, int hp, int attack, int def)
        {
            Random random = new Random();
            this.posx = posx;
            this.posy = posy;
            this.hp = hp;
            this.attack = attack;
            this.def = def;
            this.dodge = random.Next(35,43);
        }

        public static void OnTile(object source, TileEventArgs args)
        {
            Random random = new Random();
            if (args.tile == 4)
            {
                args.panel1.Show();;
                args.player.state = "enemy";
                args.backgroundworker1.RunWorkerAsync();
                switch (args.level.gamedifficulty)
                {
                    case 1:
                        args.enemy.hp = random.Next(40, 56)+ (random.Next(8,16) * (args.level.scale - 1));
                        args.enemy.attack = random.Next(20, 31) + (random.Next(3,6) * (args.level.scale - 1));
                        args.enemy.def = random.Next(40 - args.enemy.attack + 5, 40 - args.enemy.attack + 9) + (random.Next(4,7) * (args.level.scale - 1));

                        break;
                    case 2:
                        args.enemy.hp = random.Next(35, 50) + (random.Next(8, 16) * (args.level.scale - 1));
                        args.enemy.attack = random.Next(18, 28) + (random.Next(3, 6) * (args.level.scale - 1));
                        args.enemy.def = random.Next(35 - args.enemy.attack + 2, 35 - args.enemy.attack + 7) + (random.Next(4, 7) * (args.level.scale - 1));

                        break;
                    case 3:
                        args.enemy.hp = random.Next(30, 43) + (random.Next(8, 16) * (args.level.scale - 1));
                        args.enemy.attack = random.Next(13, 24) + (random.Next(3, 6) * (args.level.scale - 1));
                        args.enemy.def = random.Next(32 - args.enemy.attack, 32 - args.enemy.attack + 5) + (random.Next(4, 7) * (args.level.scale - 1));

                        break;
                    case 4:
                        args.enemy.hp = random.Next(25, 35) + (random.Next(8, 16) * (args.level.scale - 1));
                        args.enemy.attack = random.Next(7, 15) + (random.Next(3, 6) * (args.level.scale - 1));
                        args.enemy.def = random.Next(22 - args.enemy.attack, 22 - args.enemy.attack + 4) + (random.Next(4, 7) * (args.level.scale - 1));

                        break;
                }

                args.enemy.dodge = random.Next(35 + args.level.scale * 2, 43 + args.level.scale * 3);
                args.label3.Show();
                args.label3.Text = String.Format("HP: {0}", args.enemy.hp);

                switch (args.dir)
                {
                    case 87:
                        args.enemy.posx = args.player.posx;
                        args.enemy.posy = args.player.posy - 1;
                        break;
                    case 65:
                        args.enemy.posx = args.player.posx - 1;
                        args.enemy.posy = args.player.posy;

                        break;
                    case 83:
                        args.enemy.posx = args.player.posx;
                        args.enemy.posy = args.player.posy + 1;

                        break;
                    case 68:
                        args.enemy.posx = args.player.posx + 1;
                        args.enemy.posy = args.player.posy;

                        break;
                    default:
                        break;
                }


            }
        }

    }

    public class Player
    {
        public int posx;
        public int posy;
        public string state;
        public int hp;
        public int maxhp;
        public double atk;
        public int def;
        public double exp;
        public int level;
        public string name;
        public static List<Ability> learned = new List<Ability>();
        public List<string> inventory = new List<string>();
        public Image potrait;
        public Image gridicon;

        public Player(int posx, int posy, int maxhp, double atk, int def, string name)
        {
            this.posx = posx;
            this.posy = posy;
            this.atk = atk;
            this.hp = maxhp;
            this.name = name;
            this.maxhp = maxhp;
            this.def = def;
            this.state = "pohyb";
            this.exp = 0;
            this.level = 1;

        }

        public static void OnTile(object source, TileEventArgs args)
        {
            Random random = new Random();
            
            if (args.tile == 6)
            {
                if (args.player.inventory.Contains(""))
                {
                    args.player.inventory[args.player.inventory.IndexOf("")] = args.itemy[random.Next(0, args.itemy.Count)];
                    args.richtextbox1.Clear();
                    for(int i = 0; i < args.player.inventory.Count; i++)
                    {
                        if(args.player.inventory[i] != "")
                        {
                            args.richtextbox1.AppendText(String.Format("{1}: {0}." + Environment.NewLine, args.player.inventory[i], i + 1));
                        }
                        
                    }

                }
                else if (args.player.inventory.Count < 10)
                {
                    args.player.inventory.Add(args.itemy[random.Next(0, args.itemy.Count)]);
                    args.richtextbox1.AppendText(String.Format("{1}: {0}." + Environment.NewLine, args.player.inventory[args.player.inventory.Count - 1], args.player.inventory.Count));                     
                }
                else
                {
                    args.player.state = args.itemy[random.Next(0, args.itemy.Count)];
                    args.filldelegate();
                    args.label2.Show();
                    args.label2.Text = String.Format("Našel jsi {0}. Plný inventář. Vyberte předmět na výměnu. 0 = zahodit", args.player.state);
                    args.panel3.Show();
                    
                }
            }

        }


        
    }

    public class Ability
    {
        public int[] sequence { get; set; }
        public int power { get; set; }
        public string name { get; set; }
        public double accuracy { get; set; }

    }
    

    public class Level
    {
        public int[,] grid;
        public int width;
        public int height;
        public int epicentra;
        public int pocet_nepratel;
        public int pocet_truhel;
        public int exitposx;
        public int exitposy;
        public int scale;
        public int gamedifficulty;
        public int avaragesize;
        public bool repaint;

        public Level(int width, int height)
        {
            Random random = new Random();
            this.width = width;
            this.width = width;
            this.grid = new int[height, width];

            this.avaragesize = 2;
            this.epicentra = random.Next(2 + avaragesize,(avaragesize * 2) + 2);
            this.pocet_nepratel = random.Next(6, epicentra * 2 + 3);
            this.pocet_truhel = random.Next(3, epicentra + 2);
            this.scale = 1;
            this.gamedifficulty = 2;
            repaint = true;

        }

        public static void OnTile(object source, TileEventArgs args)
        {
            Random random = new Random();
            if (args.tile == 5)
            {
                args.panel1.Show(); ;
                args.player.state = "boss";
                args.backgroundworker1.RunWorkerAsync();
                switch (args.level.gamedifficulty)
                {
                    case 1:
                        args.enemy.hp = random.Next(70, 91) + (random.Next(8, 16) * (args.level.scale - 1));
                        args.enemy.attack = random.Next(35, 41) + (random.Next(3, 6) * (args.level.scale - 1));
                        args.enemy.def = random.Next(18, 26) + (random.Next(4, 7) * (args.level.scale - 1));

                        break;
                    case 2:
                        args.enemy.hp = random.Next(60, 81) + (random.Next(8, 16) * (args.level.scale - 1));
                        args.enemy.attack = random.Next(30, 36) + (random.Next(3, 6) * (args.level.scale - 1));
                        args.enemy.def = random.Next(16, 22) + (random.Next(4, 7) * (args.level.scale - 1));

                        break;
                    case 3:
                        args.enemy.hp = random.Next(50, 66) + (random.Next(8, 16) * (args.level.scale - 1));
                        args.enemy.attack = random.Next(25, 32) + (random.Next(3, 6) * (args.level.scale - 1));
                        args.enemy.def = random.Next(14, 19) + (random.Next(4, 7) * (args.level.scale - 1));

                        break;
                    case 4:
                        args.enemy.hp = random.Next(35, 50) + (random.Next(8, 16) * (args.level.scale - 1));
                        args.enemy.attack = random.Next(18, 25) + (random.Next(3, 6) * (args.level.scale - 1));
                        args.enemy.def = random.Next(12, 17) + (random.Next(4, 7) * (args.level.scale - 1));

                        break;
                }
                
                args.enemy.dodge = random.Next(30 + args.level.scale * 2, 41 + args.level.scale * 3);
                args.label3.Show();
                args.label3.Text = String.Format("HP: {0}", args.enemy.hp);

                switch (args.dir)
                {
                    case 87:
                        args.level.exitposx = args.player.posx;
                        args.level.exitposy = args.player.posy - 1;
                        break;
                    case 65:
                        args.level.exitposx = args.player.posx - 1;
                        args.level.exitposy = args.player.posy;

                        break;
                    case 83:
                        args.level.exitposx = args.player.posx;
                        args.level.exitposy = args.player.posy + 1;

                        break;
                    case 68:
                        args.level.exitposx = args.player.posx + 1;
                        args.level.exitposy = args.player.posy;

                        break;
                    default:
                        break;
                }

            }

            if(args.tile == 7)
            {
                args.panel2.Show();
            }
        }
    }

    //Zajišťuje pohyb na mapě a posílá informace o předmětech kolem hráče, pokud do nich vejde + posílá informace o zmáčknutém tlačítku.
    public class Pohyb
    {
        public delegate void TileEventHandler(object source, TileEventArgs args);
        public event TileEventHandler Tile;


        public static void Movement(Level level, Player player, int keychar, PictureBox pictureBox1, Pohyb movement, PictureBox pictureBox2, Enemy enemy, Label label3, Panel panel1, BackgroundWorker backgroundworker1, List<string> itemy, RichTextBox richtextbox1, Label label6, Panel panel2, Panel panel3, ItemFill filldelegate)
        {
            int tile = 1;

            if(level.grid[player.posx, player.posy] == 7)
            {
                tile = 7;
            }

            switch (keychar)
            {
                case 87:

                    if (level.grid[player.posx, player.posy - 1] == 1)
                    {
                        MovePlayer(player.posx, player.posy, 0, -1, pictureBox1, tile, player.gridicon);
                        level.grid[player.posx, player.posy] = tile;
                        level.grid[player.posx, player.posy - 1] = 3;
                        player.posy -= 1;
                    }
                    else if (level.grid[player.posx, player.posy - 1] == 6)
                    {
                        player.posy -= 1;
                        movement.OnTile(level.grid[player.posx, player.posy], keychar, player, level, enemy, label6, label3, panel1, backgroundworker1, itemy, richtextbox1, panel2, panel3, filldelegate);
                        MovePlayer(player.posx, player.posy + 1, 0, -1, pictureBox1, tile, player.gridicon);
                        level.grid[player.posx, player.posy + 1] = tile;
                        level.grid[player.posx, player.posy] = 3;
                        
                    }
                    else
                    {
                        movement.OnTile(level.grid[player.posx, player.posy - 1], keychar, player, level, enemy, label6, label3, panel1, backgroundworker1, itemy, richtextbox1, panel2, panel3, filldelegate);
                    }
                    break;
                case 65:
                    if (level.grid[player.posx - 1, player.posy] == 1)
                    {
                        MovePlayer(player.posx, player.posy, -1, 0, pictureBox1, tile, player.gridicon);
                        level.grid[player.posx, player.posy] = tile;
                        level.grid[player.posx - 1, player.posy] = 3;
                        player.posx -= 1;

                    }
                    else if (level.grid[player.posx - 1, player.posy] == 6)
                    {
                        player.posx -= 1;
                        movement.OnTile(level.grid[player.posx, player.posy], keychar, player, level, enemy, label6, label3, panel1, backgroundworker1, itemy, richtextbox1, panel2, panel3, filldelegate);
                        MovePlayer(player.posx + 1, player.posy, -1, 0, pictureBox1, tile, player.gridicon);
                        level.grid[player.posx + 1, player.posy] = tile;
                        level.grid[player.posx, player.posy] = 3;
                    }
                    else
                    {
                        movement.OnTile(level.grid[player.posx - 1, player.posy], keychar, player, level, enemy, label6, label3, panel1, backgroundworker1, itemy, richtextbox1, panel2, panel3, filldelegate);
                    }
                    break;
                case 83:
                    if (level.grid[player.posx, player.posy + 1] == 1)
                    {
                        MovePlayer(player.posx, player.posy, 0, 1, pictureBox1, tile, player.gridicon);
                        level.grid[player.posx, player.posy] = tile;
                        level.grid[player.posx, player.posy + 1] = 3;
                        player.posy += 1;
                    }
                    else if (level.grid[player.posx, player.posy + 1] == 6)
                    {
                        player.posy += 1;
                        movement.OnTile(level.grid[player.posx, player.posy], keychar, player, level, enemy, label6, label3, panel1, backgroundworker1, itemy, richtextbox1, panel2, panel3, filldelegate);
                        MovePlayer(player.posx, player.posy - 1, 0, 1, pictureBox1, tile, player.gridicon);
                        level.grid[player.posx, player.posy - 1] = tile;
                        level.grid[player.posx, player.posy] = 3;
                    }
                    else
                    {
                        movement.OnTile(level.grid[player.posx, player.posy + 1], keychar, player, level, enemy, label6, label3, panel1, backgroundworker1, itemy, richtextbox1, panel2, panel3, filldelegate);
                    }
                    break;
                case 68:
                    if (level.grid[player.posx + 1, player.posy] == 1)
                    {
                        MovePlayer(player.posx, player.posy, 1, 0, pictureBox1, tile, player.gridicon);
                        level.grid[player.posx, player.posy] = tile;
                        level.grid[player.posx + 1, player.posy] = 3;
                        player.posx += 1;
                        
                    }
                    else if (level.grid[player.posx + 1, player.posy] == 6)
                    {
                        player.posx += 1;
                        movement.OnTile(level.grid[player.posx, player.posy], keychar, player, level, enemy, label6, label3, panel1, backgroundworker1, itemy, richtextbox1, panel2, panel3, filldelegate);
                        MovePlayer(player.posx - 1, player.posy, 1, 0, pictureBox1, tile, player.gridicon);
                        level.grid[player.posx - 1, player.posy] = tile;
                        level.grid[player.posx, player.posy] = 3;
                    }
                    else
                    {
                        movement.OnTile(level.grid[player.posx + 1, player.posy], keychar, player, level, enemy, label6, label3, panel1, backgroundworker1, itemy, richtextbox1, panel2, panel3, filldelegate);
                    }

                    break;
                default:
                    break;

            }
        }

        private static void MovePlayer(int x, int y, int posx, int posy, PictureBox pictureBox1, int tile, Image playericon)
        {
            if (tile == 1)
            {
                Graphics g = pictureBox1.CreateGraphics();
                Image newImage = Properties.Resources.zem;
                Point point = new Point(x * 32, y * 32);

                g.DrawImage(newImage, point);
                Image newImage2 = playericon;
                Point point2 = new Point((x + posx) * 32, (y + posy) * 32);

                g.DrawImage(newImage2, point2);
            }
            else if (tile == 7)
            {
                Graphics g = pictureBox1.CreateGraphics();
                Image newImage = Properties.Resources.otevrene_dvere;
                Point point = new Point(x * 32, y * 32);

                g.DrawImage(newImage, point);
                Image newImage2 = playericon;
                Point point2 = new Point((x + posx) * 32, (y + posy) * 32);

                g.DrawImage(newImage2, point2);
            }

            
        }

        protected virtual void OnTile(int tile, int dir, Player player, Level level, Enemy enemy, Label label2, Label label3, Panel panel1, BackgroundWorker backgroundworker1, List<string> itemy, RichTextBox richtextbox1, Panel panel2, Panel panel3, ItemFill filldelegate)
        {
            Tile?.Invoke(this, new TileEventArgs(tile, dir, player, level, enemy, label2, label3, panel1, backgroundworker1, itemy, richtextbox1, panel2, panel3, filldelegate));
        }
        
    }

    public class TileEventArgs : EventArgs
    {
        public int tile;
        public int dir;
        public Player player;
        public Enemy enemy;
        public Level level;
        public Label label2;
        public Label label3;
        public Panel panel1;
        public BackgroundWorker backgroundworker1;
        public List<string> itemy;
        public RichTextBox richtextbox1;
        public Panel panel2;
        public Panel panel3;
        public ItemFill filldelegate;

        public TileEventArgs(int tile, int dir, Player player, Level level, Enemy enemy, Label label2, Label label3, Panel panel1, BackgroundWorker backgroundworker1, List<string> itemy, RichTextBox richtextbox1, Panel panel2, Panel panel3, ItemFill filldelegate)
        {
            this.tile = tile;
            this.player = player;
            this.level = level;
            this.dir = dir;
            this.enemy = enemy;
            this.label2 = label2;
            this.label3 = label3;
            this.panel1 = panel1;
            this.backgroundworker1 = backgroundworker1;
            this.itemy = itemy;
            this.richtextbox1 = richtextbox1;
            this.panel2 = panel2;
            this.panel3 = panel3;
            this.filldelegate = filldelegate;
        }

    }
    

}
