from unittest import TestCase
from precomputeBundle.client.backend.fileOperations.myMathClass import myMathClass

class TestMyMathClass(TestCase):
    global a,b

    def __init__(self, methodName='runTest'):
        TestCase.__init__(self,methodName)
        global  a,b
        a = 5
        b = 6

    def test_add(self):
        global a,b
        self.assertEqual(myMathClass.add(a,b),a+b)

    def test_sub(self):
        global a, b
        self.fail()


