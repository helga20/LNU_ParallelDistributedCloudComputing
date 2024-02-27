#include <iostream>
#include <sstream>
#include <fstream>
#include <vector>
using std::ifstream;
using namespace std;

vector<string> ReadTxtFile()
{
    std::string filename = "StartFile.txt";

    std::ifstream file(filename);
    vector<string> data;
    if (file.is_open()) 
    {
        std::string line;
        while (std::getline(file, line)) 
        {
            string n = "\n";
            //string d = line.c_str() + n;
            data.push_back(line.c_str() + n);
            //std::cout << line.c_str() << std::endl;
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


int main()
{
    
    auto d = ReadTxtFile();
    //WriteTxtFile(d);
    //WriteTxtFile(d);

}
