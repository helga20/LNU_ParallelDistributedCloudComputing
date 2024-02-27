#include <iostream>
#include <cmath>
#include <queue>
#include <future>
#include <chrono>
using namespace std;


const int N = 100;

double gCount(int i, int j, double(*g)[N][N], double(*a)[N][N])
{
	double res = (*a)[i][j];
	if (i > 0)
	{
		double count = 1;
		for (int k = i; k <= j; k++)
		{
			count *= (*g)[i - 1][k];
		}
		res -= count;
	}

	res = res / ((*g)[i][i]);
	return res;
}

double gCount(int i, double(*g)[N][N], double(*a)[N][N])
{
	double res = (*a)[i][i];

	for (int k = 0; k < i; k++)
	{
		res = res - pow((*g)[k][i], 2);
	}
	return sqrt(res);
}

double SCount(double(*g)[N][N], double S[N], int i)
{
	double St = S[i];
	for (int k = 0; k < i; k++)
	{
		St = St - (*g)[k][i] * S[k];
	}
	St = St / (*g)[i][i];
	return St;
}

double YCount(double(*g)[N][N], double Y[N], int i, double b[N])
{
	double y = b[i];
	for (int k = 0; k < i; k++)
	{
		y = y - (*g)[k][i] * Y[k];
	}
	y = y / (*g)[i][i];
	return y;
}

double XCount(double(*g)[N][N], double Y[N], double X[N], int i)
{
	double x = Y[i];
	for (int k = N-1; k > i; k--)
	{
		x = x - (*g)[i][k] * X[k];
	}
	x = x / (*g)[i][i];
	return x;
}


void DoSum(double *S, double*b, double(*a)[N][N])
{
	for (int i = 0; i < N; i++)
	{
		double sum = 0;
		for (int j = 0; j < N; j++)
		{
			sum += (*a)[i][j];
		}
		S[i] = sum + b[i];
		//cout << S[i] << "  ";
	}
}

void DoG(double(*a)[N][N], double(*g)[N][N])
{
	for (int i = 0; i < N; i++)
	{
		for (int j = 0; j < N; j++)
		{

			if (i == j)
			{
				(*g)[i][j] = gCount(i, g, a);
			}
			if (i < j)
			{
				(*g)[i][j] = gCount(i, j, g, a);
			}

			//cout << (*g)[i][j] << "  ";
		}
		//cout << "\n";
	}
}

void DoS(double(*g)[N][N], double *S, double *St)
{
	for (int i = 0; i < N; i++)
	{
		St[i] = SCount(g, S, i);
		//cout << St[i] << "  ";
	}
}

void DoY(double *Y, double *b, double(*g)[N][N])
{
	for (int i = 0; i < N; i++)
	{
		Y[i] = YCount(g, Y, i, b);
	}
}

void DoX(double *X, double *Y, double(*g)[N][N])
{
	for (int i = N - 1; i >= 0; i--)
	{
		X[i] = XCount(g, Y, X, i);
		//cout << X[i] << "  ";
	}
}


int main()
{
	/*double a[N][N] = {
		{3.2, 1, 1},
		{1, 3.7, 1},
		{1, 1, 4.2}
	};*/

	//double b[] = { 4, 4.5, 4 };

	double a[N][N] = {};
	double b[N] = {};

	srand(time(0));
	for (int i = 0; i < N; i++)
	{
		for (int j = 0; j < N; j++)
		{
			a[i][j] = 1 + (rand() % 20);
		}
		b[i] = 5 + (rand() % 20);
	}



	double S[N] = {};
	double g[N][N] = {};
	double St[N] = {};
	double Y[N] = {};
	double X[N] = {};

	chrono::steady_clock::time_point begin = chrono::steady_clock::now();
	queue<future<void>> que;
	que.push(async(std::launch::async, DoSum, S, b, &a));
	que.push(async(std::launch::async, DoG, &a, &g));
	que.push(async(std::launch::async, DoS, &g, S, St));
	


	//DoG(&a, &g);
	DoY(Y, b, &g);
	DoX(X, Y, &g);
	
	chrono::steady_clock::time_point end = chrono::steady_clock::now();


	/* << "\n\nThis is result for threads :\n\n";
	for (int i = 0; i < N; i++)
	{
		cout << X[i] << " ";
	}*/

	chrono::steady_clock::time_point begin2 = chrono::steady_clock::now();
	DoSum(S, b, &a);
	DoG(&a, &g);
	DoS(&g, S, St);
	DoY(Y, b, &g);
	DoX(X, Y, &g);
	chrono::steady_clock::time_point end2 = chrono::steady_clock::now();
	
	
	/*cout << "\n\nThis is result fo thread :\n\n";
	for (int i = 0; i < N; i++)
	{
		cout << X[i] << " ";
	}*/

	cout << "Time for one thread: " << chrono::duration_cast<chrono::microseconds>(end2 - begin2).count() << endl;
	cout << "Time for many threads:" << chrono::duration_cast<chrono::microseconds>(end - begin).count() << endl;


}

