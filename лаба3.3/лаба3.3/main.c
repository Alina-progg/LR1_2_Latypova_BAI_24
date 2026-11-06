#define _USE_MATH_DEFINES
#include <stdio.h>
#include <math.h>

int main() {

    const float k = 1.13f;
    const float x1 = 0.0f;
    const float x2 = 2 * M_PI;
    const float step = M_PI / 6.0f;

    float x = x1;    // Текущее значение x (начинаем с x1)

    // Шапка таблицы
    printf("%9s|%9s|%9s|\n", "x", "theta", "y");
    printf("------------------------------\n");

    for (float x = x1; x1 <= x2; x += step)
    {
        float theta = pow(cos(M_PI * k), 2);
        float y = sin(x) + theta - exp(x);

        // Вывод с увеличенной точностью
        printf("%9.4f|%9.4f|%9.4f|\n", x, theta, y);

    }
    return 0;
}

