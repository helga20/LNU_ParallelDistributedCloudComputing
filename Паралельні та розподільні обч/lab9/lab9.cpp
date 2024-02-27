#include <iostream>
#include <chrono>
#include <vector>
#include <thread>


static void countNumbers(std::vector<int> line, int& val)
{

	for (int i = 0; i < line.size(); i++)
	{
		if (line[i] % 2 == 1)
		{
			val++;
		}
	}
};

int main()
{
	/* Matrix creation*/
	int resultForOneThread = 0;
	int resultForManyThreads = 0;


	const int rows = 100;
	const int columns = 100;

	int matrix[rows][columns];
	
	srand(time(0));
	for (int i = 0; i < rows; i++)
	{
		for (int j = 0; j < columns; j++)
		{
			matrix[i][j] = 1 + (rand() % 20);
			//std::cout << matrix[i][j] << "  ";
		}
		//std::cout << "\n";
	}

	/* Main thread work*/


	std::chrono::steady_clock::time_point begin = std::chrono::steady_clock::now();
	

	
	for (int i = 0; i < rows; i++)
	{
		for (int j = 0; j < columns; j++)
		{
			if (matrix[i][j] % 2 == 1)
			{
				resultForOneThread++;
			}
		}
	}


	std::chrono::steady_clock::time_point end = std::chrono::steady_clock::now();
	/*Many threads work*/
	
	std::vector<std::thread> threads;

	for (int i = 0; i < rows; i++)
	{
		std::vector<int> line;

		for (int j = 0; j < columns; j++)
		{
			line.push_back(matrix[i][j]);
		}
		threads.push_back(std::thread(countNumbers, line, std::ref(resultForManyThreads)));
	}
	std::chrono::steady_clock::time_point begin2 = std::chrono::steady_clock::now();
	for (int i = 0; i < threads.size(); i++)
	{
		threads[i].join();
	}
	std::chrono::steady_clock::time_point end2 = std::chrono::steady_clock::now();
	std::cout << "\n\nResult of main thread work: "<<resultForOneThread;
	std::cout << "\nResult of many threads work: " << resultForManyThreads << std::endl;


	std::cout << "Time for one thread: " << std::chrono::duration_cast<std::chrono::microseconds>(end2 - begin2).count() << std::endl;
	std::cout << "Time for many threads:" << std::chrono::duration_cast<std::chrono::microseconds>(end - begin).count() << std::endl;


	return 0;


};