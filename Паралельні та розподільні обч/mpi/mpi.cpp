
#include <stdio.h>
#include <iostream>
#include <cstdlib>
#include "mpi.h"
using namespace std;

#define SIZE 2000
#define FROM_MASTER 1
#define FROM_WORKER 2

MPI_Status status;

static double a[SIZE][SIZE];
static double b[SIZE][SIZE];
static double c[SIZE][SIZE];

static void init_matrix()
{
	for (int i = 0; i < SIZE; i++)
	{
		for (int j = 0; j < SIZE; j++)
		{
			a[i][j] = rand() % 100 + 1;
			b[i][j] = rand() % 100 + 1;
		}
	}
}

static void print_matrix(double c[SIZE][SIZE])
{
	int i, j;
	for (i = 0; i < SIZE; i++) {
		for (j = 0; j < SIZE; j++) {
			printf(" %7.2f ", c[i][j]);
		}
		printf("\n");
	}
}

int main(int argc, char** argv)
{
	int myrank, nproc;
	int rows;
	int mtype;
	int dest, src, offseta, offsetb;
	double start_time, end_time;
	int i, j, k, l;
	MPI_Init(&argc, &argv);
	MPI_Comm_size(MPI_COMM_WORLD, &nproc);
	MPI_Comm_rank(MPI_COMM_WORLD, &myrank);

	rows = SIZE / nproc;
	mtype = FROM_MASTER;

	if (myrank == 0)
	{
		printf("SIZE = %d, number of process = %d\n", SIZE, nproc);
		init_matrix();

		start_time = MPI_Wtime();

		if (nproc == 1)
		{
			for (i = 0; i < SIZE; i++)
			{
				for (j = 0; j < SIZE; j++)
				{
					for (k = 0; k < SIZE; k++)
						c[i][j] = c[i][j] + a[i][k] * b[k][j];
				}
			}
			end_time = MPI_Wtime();
			printf("Time of work on %2d process: %f\n", nproc, end_time -
				start_time);
		}

		else
		{

			for (l = 0; l < nproc; l++) {
				offsetb = rows * l;
				offseta = rows;
				mtype = FROM_MASTER;

				for (dest = 1; dest < nproc; dest++)
				{
					MPI_Send(&offseta, 1, MPI_INT, dest, mtype, MPI_COMM_WORLD);
					MPI_Send(&offsetb, 1, MPI_INT, dest, mtype, MPI_COMM_WORLD);
					MPI_Send(&rows, 1, MPI_INT, dest, mtype, MPI_COMM_WORLD);
					MPI_Send(&a[offseta][0], rows * SIZE, MPI_DOUBLE, dest, mtype, MPI_COMM_WORLD);
					MPI_Send(&b[0][offsetb], rows * SIZE, MPI_DOUBLE, dest, mtype, MPI_COMM_WORLD);

					offseta += rows;
					offsetb = (offsetb + rows) % SIZE;

				}

				offseta = rows;
				offsetb = rows * l;

				for (i = 0; i < offseta; i++)
				{
					for (j = offsetb; j < offsetb + rows; j++)
					{
						for (k = 0; k < SIZE; k++)
						{
							c[i][j] = c[i][j] + a[i][k] * b[k][j];
						}
					}
				}

				mtype = FROM_WORKER;
				for (src = 1; src < nproc; src++) {
					MPI_Recv(&offseta, 1, MPI_INT, src, mtype, MPI_COMM_WORLD, &status);
					MPI_Recv(&offsetb, 1, MPI_INT, src, mtype, MPI_COMM_WORLD, &status);
					MPI_Recv(&rows, 1, MPI_INT, src, mtype, MPI_COMM_WORLD, &status);
					for (i = 0; i < rows; i++)
					{
						MPI_Recv(&c[offseta + i][offsetb], offseta, MPI_DOUBLE, src, mtype, MPI_COMM_WORLD, &status);
					}
				}
			}
			end_time = MPI_Wtime();
			printf("Time of work on %2d process: %f\n", nproc, end_time -
				start_time);
		}
	}

	else {

		if (nproc > 1) {
			for (l = 0; l < nproc; l++) {
				mtype = FROM_MASTER;
				MPI_Recv(&offseta, 1, MPI_INT, 0, mtype, MPI_COMM_WORLD, &status);
				MPI_Recv(&offsetb, 1, MPI_INT, 0, mtype, MPI_COMM_WORLD, &status);
				MPI_Recv(&rows, 1, MPI_INT, 0, mtype, MPI_COMM_WORLD, &status);

				MPI_Recv(&a[offseta][0], rows * SIZE, MPI_DOUBLE, 0, mtype, MPI_COMM_WORLD, &status);
				MPI_Recv(&b[0][offsetb], rows * SIZE, MPI_DOUBLE, 0, mtype, MPI_COMM_WORLD, &status);

				for (i = offseta; i < offseta + rows; i++) {
					for (j = offsetb; j < offsetb + rows; j++) {
						for (k = 0; k < SIZE; k++) {
							c[i][j] = c[i][j] + a[i][k] * b[k][j];
						}
					}

				}

				mtype = FROM_WORKER;
				MPI_Send(&offseta, 1, MPI_INT, 0, mtype, MPI_COMM_WORLD);
				MPI_Send(&offsetb, 1, MPI_INT, 0, mtype, MPI_COMM_WORLD);
				MPI_Send(&rows, 1, MPI_INT, 0, mtype, MPI_COMM_WORLD);
				for (i = 0; i < rows; i++) {
					MPI_Send(&c[offseta + i][offsetb], offseta, MPI_DOUBLE, 0,
						mtype, MPI_COMM_WORLD);

				}
			}

		}
	}

	MPI_Finalize();
	return 0;
}