import React, { useEffect, useState } from 'react';
import { AssessmentResult, fetchTrainingResults, fetchTraineeResults } from './api';
import { Users, CheckCircle, XCircle, Clock, AlertTriangle } from 'lucide-react';

function App() {
  const [results, setResults] = useState<AssessmentResult[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [selectedTrainee, setSelectedTrainee] = useState<string | null>(null);

  useEffect(() => {
    loadData();
  }, [selectedTrainee]);

  const loadData = async () => {
    setLoading(true);
    setError('');
    try {
      if (selectedTrainee) {
        const data = await fetchTraineeResults(selectedTrainee);
        setResults(data);
      } else {
        const data = await fetchTrainingResults();
        setResults(data);
      }
    } catch (err) {
      setError('Failed to load training data. Please ensure the backend is running.');
    } finally {
      setLoading(false);
    }
  };

  const totalTrainees = new Set(results.map(r => r.trainee_id)).size;
  const passedCount = results.filter(r => r.passed).length;
  const failedCount = results.filter(r => !r.passed).length;

  return (
    <div className="min-h-screen bg-gray-100 p-8">
      <div className="max-w-6xl mx-auto">
        <header className="mb-8 flex justify-between items-center">
          <div>
            <h1 className="text-3xl font-bold text-gray-900">AR Safety Dashboard</h1>
            <p className="text-gray-600">Jharkhand Mining & Manufacturing Training Metrics</p>
          </div>
          {selectedTrainee && (
            <button 
              onClick={() => setSelectedTrainee(null)}
              className="bg-blue-600 text-white px-4 py-2 rounded shadow hover:bg-blue-700"
            >
              Clear Trainee Filter
            </button>
          )}
        </header>

        {error && (
          <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-6 flex items-center">
            <AlertTriangle className="mr-2" />
            {error}
          </div>
        )}

        <div className="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
          <div className="bg-white rounded-lg shadow p-6 flex items-center">
            <Users className="text-blue-500 w-10 h-10 mr-4" />
            <div>
              <p className="text-sm text-gray-500 uppercase font-bold">Total Trainees</p>
              <p className="text-2xl font-bold">{totalTrainees}</p>
            </div>
          </div>
          <div className="bg-white rounded-lg shadow p-6 flex items-center">
            <CheckCircle className="text-green-500 w-10 h-10 mr-4" />
            <div>
              <p className="text-sm text-gray-500 uppercase font-bold">Passed</p>
              <p className="text-2xl font-bold text-green-600">{passedCount}</p>
            </div>
          </div>
          <div className="bg-white rounded-lg shadow p-6 flex items-center">
            <XCircle className="text-red-500 w-10 h-10 mr-4" />
            <div>
              <p className="text-sm text-gray-500 uppercase font-bold">Failed</p>
              <p className="text-2xl font-bold text-red-600">{failedCount}</p>
            </div>
          </div>
          <div className="bg-white rounded-lg shadow p-6 flex items-center">
            <Clock className="text-purple-500 w-10 h-10 mr-4" />
            <div>
              <p className="text-sm text-gray-500 uppercase font-bold">Completed Sessions</p>
              <p className="text-2xl font-bold">{results.length}</p>
            </div>
          </div>
        </div>

        <div className="bg-white rounded-lg shadow overflow-hidden">
          <div className="p-6 border-b border-gray-200">
            <h2 className="text-xl font-bold text-gray-800">
              {selectedTrainee ? `History for Trainee: ${selectedTrainee}` : 'Recent Training Results'}
            </h2>
          </div>
          
          {loading ? (
            <div className="p-8 text-center text-gray-500">Loading metrics...</div>
          ) : (
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Trainee ID</th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Scenario ID</th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Score</th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Mistakes</th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Duration</th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Status</th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {results.map((r, i) => (
                  <tr key={r.session_id || i} className="hover:bg-gray-50 transition-colors">
                    <td className="px-6 py-4 whitespace-nowrap">
                      <button 
                        onClick={() => setSelectedTrainee(r.trainee_id)}
                        className="text-blue-600 hover:underline font-medium"
                      >
                        {r.trainee_id}
                      </button>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">{r.scenario_id}</td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm font-semibold">{r.score}</td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-red-500">{r.mistakes}</td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{r.duration_seconds}s</td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <span className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full ${r.passed ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'}`}>
                        {r.passed ? 'Passed' : 'Failed'}
                      </span>
                    </td>
                  </tr>
                ))}
                {results.length === 0 && (
                  <tr>
                    <td colSpan={6} className="px-6 py-8 text-center text-gray-500">
                      No records found.
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          )}
        </div>
      </div>
    </div>
  );
}

export default App;
