using StarfieldSimulation.Models;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace StarfieldSimulation
{
    public partial class Form1 : Form
    {
        // Кол-во звёзд
        private const int StarCount = 15000;

        // Минимальное расстояние Z (не может быть 0 — деление!)
        private const int MinZ = 1;

        // Скорость перемещения звезды к наблюдателю
        private const int StarSpeed = 30;

        // Максимальный размер звезды (когда она ближе всего)
        private const float StarBaseSize = 10; 

        private Star[] _stars = new Star[StarCount];
        private Random _random = new Random();
        private Graphics _graphics;

        public Form1()
        {
            InitializeComponent();
        }

        // Обновляет звёзды каждый кадр
        private void timer1_Tick(object sender, EventArgs e)
        {
            // Очищаем кадр — заливаем чёрным
            _graphics.Clear(Color.Black);

            // Для каждой звезды: рисуем -> двигаем
            foreach (var star in _stars)
            {
                DrawStar(star);
                MoveStar(star);
            }

            // Обновляем изображение на PictureBox
            pictureBox1.Refresh();
        }

        // Двигает звезду к наблюдателю
        private void MoveStar(Star star)
        {
            // Z уменьшается → звезда приближается
            star.Z -= StarSpeed;

            // Когда Z стала меньше минимума — звезда "перелетела" камеру
            if (star.Z < MinZ)
            {
                // Перегенерируем позицию звезды "вдали"
                star.X = _random.Next(-pictureBox1.Width, pictureBox1.Width);
                star.Y = _random.Next(-pictureBox1.Height, pictureBox1.Height);

                // Новый Z — случайный, звезда появляется далеко
                star.Z = _random.Next(MinZ, pictureBox1.Width);
            }
        }

        // Рисует одну звезду
        private void DrawStar(Star star)
        {
            // Чем меньше Z (чем ближе звезда) — тем больше её размер.
            // Map переводит Z из диапазона [MinZ..Width] → [StarBaseSize..0]
            float starSize = Map(
                star.Z,
                MinZ, pictureBox1.Width,     // входной диапазон
                StarBaseSize, 0              // выходной диапазон
            );

            // Эффект перспективы — делим координаты на Z.
            // Звезда "разлетается" от центра экрана.
            float projectedX = star.X / star.Z;
            float projectedY = star.Y / star.Z;

            // Переводим [-1..1] → [0..Width/Height], добавляя центр экрана
            float screenX = Map(projectedX, -1, 1, 0, pictureBox1.Width);
            float screenY = Map(projectedY, -1, 1, 0, pictureBox1.Height);

            // Рисуем звезду
            _graphics.FillEllipse(
                Brushes.GreenYellow,
                screenX,
                screenY,
                starSize,
                starSize
            );
        }

        //Инициализация формы и генерация звёзд
        private void Form1_Load(object sender, EventArgs e)
        {
            // Создаём bitmap и graphics для рисования
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            _graphics = Graphics.FromImage(pictureBox1.Image);

            // Инициализация массива звёзд
            for (int i = 0; i < _stars.Length; i++)
            {
                _stars[i] = new Star
                {
                    // Начальные координаты вокруг центра
                    X = _random.Next(-pictureBox1.Width, pictureBox1.Width),
                    Y = _random.Next(-pictureBox1.Height, pictureBox1.Height),

                    // Z — расстояние до наблюдателя
                    Z = _random.Next(MinZ, pictureBox1.Width)
                };
            }

            timer1.Start();
        }


        // Универсальная функция перевода значения из одного диапазона в другой
        private float Map(float value, float inMin, float inMax, float outMin, float outMax)
        {
            return ((value - inMin) / (inMax - inMin)) * (outMax - outMin) + outMin;
        }
    }
}
