#include <ctime>
#include <iostream>
#include <mutex>
#include <random>
#include <iomanip>
#include <string>
#include <future>
#include <queue>
#include <map>
#include <thread>
#include <vector>
#include <regex>
#include <fstream>
#include <iterator>

constexpr int MAX_SIZE = 10000;
constexpr int ROUND_TO = 2;
constexpr int MAX_WORD_COUNT_IN_FILE = 100;

using std::rand, std::vector, std::map,std::cout, std::setw, std::string, std::left, std::async, std::launch,
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
		((cout << left << setw(spaces[i++]) << args << "|"), ...);
		cout << '\n';
	}
};

std::mutex read_locker, print_locker, run_locker;

class Generator
{
	string random_word(int size)
	{
		string alphabet = "abcdefghijklmnopqrstuvwxyz";
		string result;
		for(int i = 0; i < size; i++)
			result += alphabet[rand() % alphabet.size()];
		return result;
	}
public:
	Generator()
	{
		srand(time(nullptr));
	}
	vector<string> generate_files(int count)
	{
		vector<string> filenames;
		for(int i = 0; i < count; i++)
		{
			string filename = "example_file" + std::to_string(i) + ".txt";
			filenames.push_back(filename);

			std::ofstream file(filename);
			std::ostream_iterator<std::string> iterator(file, " ");
			auto text = generate_vocabulary(rand() % MAX_WORD_COUNT_IN_FILE);
			std::copy(text.begin(), text.end(), iterator);
		}
		return filenames;
	}

	vector<string> generate_vocabulary(int size)
	{
		vector<string> result(size);
		for(int i = 0; i < size; i++)
			result[i] = random_word(rand() % 100);
		return result;
	}
};
vector<string> read_from_file(string filename)
{
	read_locker.lock();
	std::ifstream file(filename);
	string text((std::istreambuf_iterator<char>(file)), std::istreambuf_iterator<char>());
	file.close();
	read_locker.unlock();
	vector<string> result;
	string current_word;
	for(char c: text)
	{
        if(regex_match(string(1, c), std::regex("[a-zA-Z0-9]")))
        {
            current_word += tolower(c);
            continue;
        }
		if(current_word != "")
			result.push_back(current_word);
		current_word = "";
	}
	if(current_word != "")
		result.push_back(current_word);
	return result;
}
map<string, int> get_words_count_for_vector(int i, vector<string>& vocabulary, vector<vector<string>> files, bool test = false)
{
	map<string, int> result;
	for(const auto& file: files)
		for(const auto& voc_word: vocabulary)
			for(const auto& file_word: file)
			result[voc_word] += (voc_word == file_word);

	if(!test)
	{
		print_locker.lock();
		cout << "End for thread with id " << std::this_thread::get_id() << " and position " << i << '\n';
		print_locker.unlock();
	}
	return result;
}
map<string, int> get_words_count_for_file(int i, vector<string> vocabulary, vector<string>filenames, bool test = false)
{
	vector<vector<string>> files;
	for(auto& filename: filenames)
		files.push_back(read_from_file(filename));
	return get_words_count_for_vector(i, vocabulary, files, test);
}
void get_words_count(vector<vector<string>> vocabularies, vector<string> filenames, bool test = false, bool sync = false)
{
	vector<map<string, int>> result(vocabularies.size());
    vector<vector<string>> text;

	for(auto& filename: filenames)
		text.push_back(read_from_file(filename));


	vector<std::function<std::future<map<string, int>>()>> create_futures;
	for(int i = 0; i < vocabularies.size(); i++)
		create_futures.push_back(

			[&text, &vocabularies, &test, i]()
			{
				return async(launch::async | launch::deferred,
				[&text, &test](int i, vector<string> vocabulary)
				{
					return get_words_count_for_vector(i, vocabulary, text, test);
				}, i, vocabularies[i]);
			}
		);
	vector<std::future<map<string, int>>> futures(create_futures.size());
	for(int i = create_futures.size() - 1; i >= 0; i--)
	{
		run_locker.lock();
		futures[i] = create_futures[i]();
		if(sync)
			futures[i].wait();
		run_locker.unlock();
	}
}

int main()
{
	with_time<get_words_count> get_time;
	Generator generator;


	TablePrint table({7, 5, 9, 11, 13});
	table.print_line();
	table.print("threads", "size", "sync time", "thread time", "acceleration");
	auto filenames = generator.generate_files(4);
	for(int thread_count = 2; thread_count <= 256; thread_count *= 2)
		for(int i = 10; i <= MAX_SIZE; i *= 10)
		{
			vector<vector<string>> vocabularies;
			for(int j = 0; j < thread_count; j++)
				vocabularies.push_back(generator.generate_vocabulary(i));
			int sync = get_time(vocabularies, filenames, true, true);
			int thread = get_time(vocabularies, filenames, true, false);
			double acceleration = round(double(sync) / std::max(1, thread));
			table.print(thread_count, i, sync, thread, acceleration);
		}
}
/*

---------------------------------------------------
|threads|size |sync time|thread time|acceleration |
|2      |10   |1479657  |1478662    |1.01         |
|2      |100  |1503425  |1498244    |1.01         |
|2      |1000 |1711347  |1604202    |1.07         |
|2      |10000|4456070  |3004662    |1.49         |
|4      |10   |1483316  |1483441    |1            |
|4      |100  |1508787  |1481311    |1.02         |
|4      |1000 |1939854  |1600839    |1.22         |
|4      |10000|7359033  |3078052    |2.4          |
|8      |10   |1485846  |1496468    |1            |
|8      |100  |1554910  |1497678    |1.04         |
|8      |1000 |2414777  |1696143    |1.43         |
|8      |10000|13399361 |4081015    |3.29         |
|16     |10   |1499915  |1484889    |1.02         |
|16     |100  |1618955  |1510639    |1.08         |
|16     |1000 |3315562  |1901564    |1.75         |
|16     |10000|24983365 |6711127    |3.73         |
|32     |10   |1513136  |1488704    |1.02         |
|32     |100  |1752428  |1541218    |1.14         |
|32     |1000 |5146035  |2272230    |2.27         |
|32     |10000|48523420 |11802756   |4.12         |
|64     |10   |1535884  |1491774    |1.03         |
|64     |100  |2031824  |1607884    |1.27         |
|64     |1000 |8790119  |3071834    |2.87         |
---------------------------------------------------
*/
