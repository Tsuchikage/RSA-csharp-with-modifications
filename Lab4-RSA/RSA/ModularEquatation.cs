using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class ModularEquatation{
    private readonly int _a;
    private readonly int _b;
    private readonly int _n;

    public ModularEquatation(int a, int b, int n){
     
     if(a <=0 || n <=0)
     throw new ArgumentException("Invalid arguments");
        _a = a;
        _b = b;
        _n = n;
    }

    public (int x, int y, int z) EuclideanAlgorithm(int a, int b) {
        if(b == 0)
        return (a, 1, 0);
        (int d1, int x1, int y1) = EuclideanAlgorithm(b, a % b);  
        int d = d1;
        int x = y1;
        int y = x1 - a / b * y1;
        return(d, x, y);
    }

    public bool HasSolution(){
        return _b % EuclideanAlgorithm(_a, _n).x == 0;
    }

    public int GetSolutionNumber(){
        return HasSolution() ? EuclideanAlgorithm(_a, _n).x : 0;
    }

    public IEnumerable<Solution> GetSolutions(){
        if(!HasSolution())
        throw new InvalidOperationException("No solutions");
        return YieldSolutions();
    }

    private IEnumerable<Solution> YieldSolutions(){
        (int d, int x1, _) = EuclideanAlgorithm(_a, _n);
        int x0 = x1 * (_b /d) % _n;
        if(x0 < 0)
        x0 += _n;
        for (int i = 0; i<= d-1; i++){
            int solution = x0 + i * (_n / d) % _n;
            if(solution > _n)
            solution -= _n;
            yield return new Solution(solution, _n);
        }
    }

    public class Solution{
        public Solution (int value, int modulo){
            Value = value;
            Modulo = modulo;
        }
        public int Value{get;}
        public int Modulo{get;}

        public override string ToString()
        {
            return Value + " (mod " + Modulo + ")";
        }
    }

}