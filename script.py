# data and output files are referenced via paths stored in local files
# local files are ignored in Git for privacy

dataPathFile = open('data.akfpath', 'rt')
dataPath = dataPathFile.readline()
dataPathFile.close()

dataFile = open(dataPath, 'rt')
data = dataFile.readlines()
dataFile.close()

outputPathFile = open('output.akfpath', 'rt')
outputPath = outputPathFile.readline()
outputPathFile.close()

with open(outputPath, 'wt') as outputFile:
    
