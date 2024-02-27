#include <ios>
#include <iostream>
#include <chrono>
#include <numeric>
#include <string>
#include <vector>
// #include <iomanip>
using std::cout, std::vector,
	  std::chrono::high_resolution_clock, std::chrono::microseconds, std::chrono::duration_cast;


double round(double x)
{
	int k = std::pow(10, 2);
	return std::ceil(x * k) / k;
}

class TablePrint
{
	vector<int> spaces;
	int count = 0;
public:
	TablePrint(std::initializer_list<int> spaces_list)
	{
		this->spaces = spaces_list;
		this->count = std::accumulate(spaces.begin(), spaces.end(), 1 + spaces.size());
	}
	void print_line()
	{
		for(int i = 0; i < count; i++)
			cout << "-";
		cout << '\n';
	}
	template< typename ... ARGS>
	void print(ARGS&& ... args)
	{
		int i = 0;
		cout << "|";
		((cout << args << "|"), ...);
		cout << '\n';
	}
};



__global__ void sum_of_element_in_row(int* matrix, int rows, int columns)
{
	int block_id = threadIdx.x + blockIdx.x * blockDim.x;

	int* result;
	for(int i = 0; i < rows; i++)
	{
		int sum = 0;
		for(int j = 0; j < columns; j++)
			sum += matrix[i * columns + j + block_id];
		// result[i] = sum;
	}
}


void random_matrix(int* matrix, int rows_count, int columns_count)
{
	for(int i = 0; i < rows_count; i++)
		for(int j = 0; j < columns_count; j++)
			matrix[i * columns_count + j] = rand() % 100;
}
int get_time(int rows_count, int columns_count, int threads_count = 1)
{
	int* matrix;
	cudaMallocHost((void **) &matrix, sizeof(int) * rows_count * columns_count);


	random_matrix(matrix, rows_count, columns_count);


	auto start = high_resolution_clock::now();

	sum_of_element_in_row<<<1, threads_count>>>(matrix, rows_count, columns_count);
	cudaDeviceSynchronize();

	auto stop = high_resolution_clock::now();

	cudaFree(matrix);
	return duration_cast<microseconds>(stop - start).count();
}
int main()
{
	TablePrint table({7, 5, 9, 11, 13});
	table.print("threads", "size", "sync time", "thread time", "acceleration");

	for(int thread_count = 2; thread_count <= 256; thread_count *= 2)
		for(int i = 10; i <= 10000; i *= 10)
		{
			int sync = get_time(i, i, 1);
			int thread = get_time(i, i, thread_count);
			double acceleration = round(double(sync) / std::max(1, thread));
			table.print(thread_count, i, sync, thread, acceleration);
		}

}
