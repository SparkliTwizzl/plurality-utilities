from setuptools import setup, find_packages

setup (
  name='petrichorscriptlexer',
  packages=find_packages(),
  entry_points =
  """
  [pygments.lexers]
  petrichorscriptlexer = petrichorscriptlexer.lexer:PetrichorScriptLexer
  """,
)
