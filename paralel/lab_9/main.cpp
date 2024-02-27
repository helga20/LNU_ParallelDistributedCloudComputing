#include <iostream>
#include <vector>
#include <random>
#include <utility>
#include <thread>
#include <chrono>
#include <iomanip>
#include <string>

constexpr int MAX_SIZE = 10000;
constexpr int ROUND_TO = 2;

using std::pair, std::thread, std::rand, std::vector,
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
int** generate_matrix(pair<int, int> size)
{
	int** result = new int*[size.first];
	for(int i = 0; i < size.first; i++)
	{
		result[i] = new int[size.second];
		for(int j = 0; j < size.second; j++)
			result[i][j] = rand() * 1000;
	}
	return result;
}

int get_even_count(int** matrix, pair<int, int> size, int thread_count, bool test=false)
{
	vector<thread> threads;
	int per_thread = size.first / thread_count;
	int ans = 0;
	if(!per_thread)
		per_thread = size.first;
	for(int i = 0; i < size.first; i+=per_thread)
	{
		threads.emplace_back(
		[matrix, size, &ans](int left, int right)
		{
			for(int i = left; i < right; i++)
				for(int j = 0; j < size.second; j++)
				{
					ans += (matrix[i][j] / 2 * 2 == matrix[i][j]);
				}

		}, i, min(size.first, i + per_thread));
	}
	for(auto& thread: threads)
		thread.join();
	return ans;
}

int main()
{
	int** matrix = new int*[3]{
		new int[3]{1, 2, 3},
		new int[3]{4, 5, 6},
		new int[3]{7, 8, 9}
	};
	pair<int, int> size = {3, 3};
	cout << 4 << ' ' <<  get_even_count(matrix, size, 1) << ' ' << get_even_count(matrix, size, 2) << '\n';
	with_time<get_even_count> even_time;

	TablePrint table({7, 5, 9, 11, 13});
	cout << "__________________________________________________\n";
	table.print("threads", "size", "sync time", "thread time", "acceleration");
	for(int thread_count = 2; thread_count <= 256; thread_count *= 2)
		for(int i = 10; i <= MAX_SIZE; i *= 10)
		{
			pair<int, int> size = {i, i};
			auto matrix = generate_matrix(size);
			int sync = even_time(matrix, size, 1, true);
			int thread = even_time(matrix, size, thread_count, true);
			double acceleration = round(double(sync) / max(1, thread));
			table.print(thread_count, i, sync, thread, acceleration);
		}
}
/*
	__________________________________________________
	threads|size |sync time|thread time|acceleration |
	2      |10   |41       |56         |0.74         |
	2      |100  |107      |80         |1.34         |
	2      |1000 |6079     |4461       |1.37         |
	2      |10000|366214   |260085     |1.41         |
	4      |10   |31       |105        |0.3          |
	4      |100  |56       |49         |1.15         |
	4      |1000 |3692     |2932       |1.26         |
	4      |10000|356125   |331362     |1.08         |
	8      |10   |98       |217        |0.46         |
	8      |100  |57       |138        |0.42         |
	8      |1000 |3872     |2750       |1.41         |
	8      |10000|357298   |244739     |1.46         |
	16     |10   |84       |28         |3            |
	16     |100  |58       |282        |0.21         |
	16     |1000 |4365     |2835       |1.54         |
	16     |10000|397889   |254050     |1.57         |
	32     |10   |53       |17         |3.12         |
	32     |100  |58       |526        |0.12         |
	32     |1000 |3711     |3171       |1.18         |
	32     |10000|382977   |243588     |1.58         |
	64     |10   |30       |18         |1.67         |
	64     |100  |89       |1578       |0.06         |
	64     |1000 |3888     |3034       |1.29         |
	64     |10000|366766   |246539     |1.49         |
	128    |10   |24       |15         |1.6          |
	128    |100  |57       |50         |1.14         |
	128    |1000 |3724     |3613       |1.04         |
	128    |10000|624777   |460835     |1.36         |
	256    |10   |57       |41         |1.4          |
	256    |100  |104      |94         |1.11         |
	256    |1000 |6403     |17938      |0.36         |
	256    |10000|361856   |244178     |1.49         |
 */
