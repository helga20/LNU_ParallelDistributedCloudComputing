#include <iostream>
#include <sstream>
#include <fstream>
#include <vector>
#include <thread>
using std::ifstream;
using namespace std;
vector<string> ReadTxtFile()
{
    std::string filename = "StartFile.txt";

    std::ifstream file(filename);
    vector<string> data;
    int i = 1;
    if (file.is_open())
    {
        std::string line;
        while (std::getline(file, line))
        {
            data.push_back(line.c_str());  
        }
        file.close();
        
    }
    return data;
}

void WriteTxtFile(vector<string> data)
{

    std::string filename = "EndFile.txt";

    std::ofstream MyFile(filename);
    for (int i = 0; i < data.size(); i++)
    {
        MyFile << data[i];
    }

    MyFile.close();
}

void WriteTxtFilet(string &data, vector<string>& d)
{
    auto k = std::hash<std::thread::id>{}(std::this_thread::get_id());   
    d.push_back(data + " потік  " + std::to_string(k) + "\n");
}

void Count(string &s)
{
    int res = 0;
    string result;
    for (int i = 0; i < s.size(); i++)
    {
        if (s[i] != ' ')
        {
            res++;
        }
        else
        {
            result += to_string(res) + " ";
            res = 0;
        }
        
    }
    s = result;
}

void threadWrite(vector<string> data)
{
    vector<string> d;
    std::vector<std::thread> threads;
    for (int i = 0; i < data.size(); i++)
    {
        threads.push_back(std::thread(WriteTxtFilet, ref(data[i]), ref(d)));
    }
    chrono::steady_clock::time_point begin = chrono::steady_clock::now();
    for (int i = 0; i < threads.size(); i++)
    {
        threads[i].join();
    }
    chrono::steady_clock::time_point end = chrono::steady_clock::now();
    cout << "Time for many threads:" << chrono::duration_cast<chrono::microseconds>(end - begin).count() << endl; WriteTxtFile(d);
    
}


int main()
{
    auto d = ReadTxtFile();

    for (int i = 0; i < d.size(); i++)
    {
        Count(ref(d[i]));
        //cout << d[i];
    }

    chrono::steady_clock::time_point begin = chrono::steady_clock::now();
    WriteTxtFile(d);
    chrono::steady_clock::time_point end = chrono::steady_clock::now();
    cout << "Time for one thread:" << chrono::duration_cast<chrono::microseconds>(end - begin).count() << endl;
    threadWrite(d);
    
}
