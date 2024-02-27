#include <mpi.h>
#include <stdio.h>
#include <stdlib.h>
#include <algorithm>
#include <unistd.h>
#include <chrono>
#include <iostream>

using namespace std;
using namespace std::chrono;

constexpr int mod_to = 1e9 + 7;

int factorial(int left, int right)
{
	int result = 1;
	for(int i = left + 1; i <= right; i++)
		result = (result * i) % mod_to;

	return result;
}
int factorial_main_process(int number)
{
	int process_count = 2;
	int process_id;

	MPI_Status status;
	MPI_Comm_size(MPI_COMM_WORLD, &process_count);

	auto start = high_resolution_clock::now();
	int per_process = number / process_count + (number % process_count != 0);
	if(process_count > 1)
	{
		for(int i = 1; i < process_count; i++)
		{
			int left = i * per_process;
			int right = std::min(left + per_process, number);

			MPI_Send(&left, 1, MPI_INT, i, 0, MPI_COMM_WORLD);
			MPI_Send(&right, 1, MPI_INT, i, 0, MPI_COMM_WORLD);
		}
	}
	int result = factorial(0, per_process);
	for(int i = 1; i < process_count; i++)
	{
		int val;
		MPI_Recv(&val, 1, MPI_INT, MPI_ANY_SOURCE, 0, MPI_COMM_WORLD, &status);
		result = (result * val) % mod_to;
	}
	auto stop = high_resolution_clock::now();
	cout << number << ' ' << duration_cast<microseconds>(stop - start).count()  << '\n';
	return result;
}
int factorial_sub_process()
{
	MPI_Status status;
	int left, right;
	MPI_Recv(&left, 1, MPI_INT, 0, 0, MPI_COMM_WORLD, &status);
	MPI_Recv(&right, 1, MPI_INT, 0, 0, MPI_COMM_WORLD, &status);
	int result = factorial(left, right);
	MPI_Send(&result, 1, MPI_INT, 0, 0, MPI_COMM_WORLD);
}

int delegate(int number, int argc, char* argv[])
{
	int process_id;

	MPI_Comm_rank(MPI_COMM_WORLD, &process_id);
	if(process_id == 0)
		return factorial_main_process(number);
	factorial_sub_process();
	return -1;
}

int main(int argc, char* argv[])
{

	// MPI_Init(&argc, &argv);
	//
	// int result = delegate(5, argc, argv);
	// if(result != -1)
	// 	cout << 120 << ' ' << result << '\n';
	//
	// MPI_Finalize();

	MPI_Init(&argc, &argv);

	for(int number = 10; number < 1000000000; number *= 10)
		delegate(number, argc, argv);

	MPI_Finalize();
}

// clear; mpic++ main.cpp -o run; mpirun -np 4 ./run;
/*
2 process
10 4
100 1
1000 5
10000 44
100000 436
1000000 4384
10000000 43687
100000000 435760

4 process
10 82
100 2
1000 4
10000 25
100000 218
1000000 2230
10000000 22138
100000000 220577
 

100
10 125018
100 1684
1000 729
10000 527
100000 705
1000000 1331
10000000 10965
100000000 85806
*/
