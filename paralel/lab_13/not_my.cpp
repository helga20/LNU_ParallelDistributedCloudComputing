#include <iostream>
#include <chrono>
#include <cstdlib>
#include "mpi.h"

#pragma region DECLARATIONS

int** generate_matrix(const int&, const int&);

#pragma endregion DECLARATIONS


int main(
	int argc,
	char** argv)
{
	const int n = 1000;

	int** matrix = generate_matrix(n, n);
	int matrix_temp[256][256];
	int sum = 0;
	int i = 0;
	int elems_received = 0;

	int proc = 0;
	int rank = 0;

	double start_time = 0;
	double end_time = 0;

	MPI_Init(&argc, &argv);
	MPI_Comm_size(MPI_COMM_WORLD, &proc);
	MPI_Comm_rank(MPI_COMM_WORLD, &rank);
	MPI_Status status;

	int row_index = 0;
	int rows = n / proc;

	if (rank == 0)
	{
		start_time = MPI_Wtime();

		if (proc > 1)
		{
			for (i = 1; i < proc - 1; ++i)
			{
				row_index = i * rows;
				MPI_Send(&rows, 1, MPI_INT, i, 0, MPI_COMM_WORLD);
				MPI_Send(&matrix[row_index][0], rows, MPI_INT, i, 0, MPI_COMM_WORLD);
			}

			row_index = i * rows;
			int rows_left = n - row_index;

			MPI_Send(&rows_left, 1, MPI_INT, i, 0, MPI_COMM_WORLD);
			MPI_Send(&matrix[row_index][0], rows_left, MPI_INT, i, 0, MPI_COMM_WORLD);
		}
			
		for (i = 0; i < rows; ++i)
		{
			for (int j = 0; j < n; ++j)
			{
				if (matrix[i][j] % 3 == 0)
				{
					sum += matrix[i][j];
				}
			}
		}

		int temp = 0;
		for (i = 1; i < proc; ++i)
		{
			MPI_Recv(&temp, 1, MPI_INT, MPI_ANY_SOURCE, 0, MPI_COMM_WORLD, &status);
			int sender = status.MPI_SOURCE;
			sum += temp;
		}

		end_time = MPI_Wtime();
		std::cout << "Time taken: " << end_time - start_time << std::endl;
		std::cout << sum;
	}
	else
	{
		MPI_Recv(&elems_received, 1, MPI_INT, 0, 0, MPI_COMM_WORLD, &status);
		MPI_Recv(&matrix_temp, elems_received, MPI_INT, 0, 0, MPI_COMM_WORLD, &status);
		
		int partial_sum = 0;
		for (int i = 0; i < elems_received; ++i)
		{
			for (int j = 0; j < n; ++j)
			{
				if (matrix_temp[i][j] % 3 == 0)
				{
					partial_sum += matrix_temp[i][j];
				}
			}
		}

		MPI_Send(&partial_sum, 1, MPI_INT, 0, 0, MPI_COMM_WORLD);
	}
	
	MPI_Finalize();
	return (EXIT_SUCCESS);
}


#pragma region DEFINITIONS

int** generate_matrix(
	const int& n,
	const int& m)
{
	using namespace std;
	int** matrix = new int* [n];
	const int MIN = 10;
	const int MAX = 30;

	for (int i = 0; i < n; ++i)
	{
		matrix[i] = new int[m];
		for (int j = 0; j < m; ++j)
		{
			matrix[i][j] = rand() % (MAX - MIN) + MIN;
		}
	}

	return (matrix);
}

#pragma endregion DEFINITIONS
