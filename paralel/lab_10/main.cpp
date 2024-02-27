#include <iostream>
#include <random>
#include <chrono>
#include <iomanip>
#include <string>
#include <future>
#include <queue>

constexpr int MAX_SIZE = 10000;
constexpr int ROUND_TO = 2;

using std::rand, std::vector, std::async, std::launch, std::queue,
	  std::min, std::cout, std::setw, std::string, std::left, std::max,
	  std::chrono::high_resolution_clock, std::chrono::microseconds, std::chrono::duration_cast;

double round(double x)
{
	int k = std::pow(10, ROUND_TO);
	return std::ceil(x * k) / k;
}

template<auto FUNC>
struct with_time
{
	template<typename ... ARGS>
	int operator()(ARGS ... args)
	{
		auto start = high_resolution_clock::now();
		FUNC(args...);
		auto stop = high_resolution_clock::now();
		return duration_cast<microseconds>(stop - start).count();
	}
};

struct TablePrint
{
	vector<int> spaces;
	TablePrint(std::initializer_list<int> spaces_list)
	{
		this->spaces = spaces_list;
	}
	template< typename ... ARGS>
	void print(ARGS&& ... args)
	{
		int i = 0;
		((cout << left << setw(spaces[i++]) << args << "|"), ...);
		cout << '\n';
	}
};

template<typename Iter_T>
long double norm(Iter_T first, Iter_T last)
{
  return sqrt(std::inner_product(first, last, first, 0.0L));
}

double* substruct(double* arr1, double* arr2, int size)
{
	double* result = new double[size];
	for(int i = 0; i < size; i++)
		result[i] = arr1[i] - arr2[i];
	return result;
}
double multiply(double* vector_1, double* vector_2, int size)
{
	double result;
	for(int i = 0; i < size; i++)
		result += round(vector_1[i] * vector_2[i]);
	return result;
}
double* multiply(double** matrix, double* vector, int size, int thread_count = 1)
{
	double* result = new double[size];
	int per_thread = size / thread_count;
	if(!per_thread)
		per_thread = size;
	queue<std::future<void>> futures_queue;
	for (int i = 0; i < size; i += per_thread)
		futures_queue.push(async(launch::async | launch::deferred,
		[matrix, &result, vector, size](int left, int right)
		{
			for(int j = left; j < right; j++)
				result[j] = multiply(matrix[j], vector, size);
		}, i, min(size, i + per_thread)));

	return result;
}
double* relaxation_method(double** A, double* b, double omega, int size, double esp, int thread_count = 1)
{
	double* result = new double[size];
	double current_esp = esp + 1;
	for(int i = 0; i < size; i++)
		result[i] = 0;
	while(current_esp > esp)
	{
		current_esp = 0;
		for(int i = 0; i < size; i++)
		{
			double sigma = 0;
			for(int j = 0; j < size; j++)
				if(i != j)
					sigma += A[i][j] * result[j];
			result[i] = (1 - omega) * result[i] + (omega / A[i][i]) * (b[i] - sigma);
		}
		auto vector = substruct(multiply(A, result, size, thread_count), b, size);
		current_esp = norm(vector, vector + size);
	}
	return result;
}

double** generate_matrix(int size)
{
	double** result = new double*[size];
	for(int i = 0; i < size; i++)
	{
		result[i] = new double[size];
		for(int j = 0; j < size; j++)
			result[i][j] = rand();
	}
	return result;
}
double* generate_vector(int size)
{
	double* result = new double[size];
	for(int i = 0; i < size; i++)
		result[i] = rand();
	return result;
}

int main()
{
	with_time<relaxation_method> get_time;

	TablePrint table({7, 5, 9, 11, 13});
	cout << "__________________________________________________\n";
	table.print("threads", "size", "sync time", "thread time", "acceleration");
	for(int thread_count = 2; thread_count <= 256; thread_count *= 2)
		for(int i = 10; i <= MAX_SIZE; i *= 10)
		{
			auto matrix = generate_matrix(i);
			auto vector = generate_vector(i);
			int sync = get_time(matrix, vector, 0.5, i, 0.001, 1);
			int thread = get_time(matrix, vector, 0.5, i, 0.001, thread_count);
			double acceleration = round(double(sync) / max(1, thread));
			table.print(thread_count, i, sync, thread, acceleration);
		}
}
/*
	__________________________________________________
	threads|size |sync time|thread time|acceleration |
	2      |10   |26460    |79         |334.94       |
	2      |100  |6541     |160        |40.89        |
	2      |1000 |14578    |9813       |1.49         |
	2      |10000|814968   |539473     |1.52         |
	4      |10   |50       |120        |0.42         |
	4      |100  |3392     |107        |31.71        |
	4      |1000 |8965     |5185       |1.73         |
	4      |10000|769405   |365725     |2.11         |
	8      |10   |81       |226        |0.36         |
	8      |100  |3422     |213        |16.07        |
	8      |1000 |32796    |4100       |8            |
	8      |10000|754270   |358613     |2.11         |
	16     |10   |98       |30         |3.27         |
	16     |100  |169      |385        |0.44         |
	16     |1000 |31982    |4253       |7.52         |
	16     |10000|854786   |404010     |2.12         |
	32     |10   |122      |86         |1.42         |
	32     |100  |118      |683        |0.18         |
	32     |1000 |24596    |4201       |5.86         |
	32     |10000|790490   |357174     |2.22         |
	64     |10   |186      |105        |1.78         |
	64     |100  |480      |2236       |0.22         |
	64     |1000 |25155    |4851       |5.19         |
	64     |10000|782438   |373005     |2.1          |
	128    |10   |113      |25         |4.52         |
	128    |100  |117      |109        |1.08         |
	128    |1000 |8155     |6541       |1.25         |
	128    |10000|778801   |372879     |2.09         |
	256    |10   |51       |22         |2.32         |
	256    |100  |230      |125        |1.84         |
	256    |1000 |8795     |10574      |0.84         |
	256    |10000|758024   |363607     |2.09         |
 */
