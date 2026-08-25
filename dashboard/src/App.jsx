import React from 'react';
import { ShieldAlert, Users, CheckCircle, XCircle, Clock } from 'lucide-react';

const mockData = [
  { id: '1', trainee_id: 'T001', scenario_id: 'gas_leak', score: 86, mistakes: 2, duration_seconds: 142, passed: true },
  { id: '2', trainee_id: 'T002', scenario_id: 'fire_explosion', score: 95, mistakes: 0, duration_seconds: 90, passed: true },
  { id: '3', trainee_id: 'T003', scenario_id: 'gas_leak', score: 45, mistakes: 4, duration_seconds: 210, passed: false },
  { id: '4', trainee_id: 'T001', scenario_id: 'fire_explosion', score: 88, mistakes: 1, duration_seconds: 120, passed: true },
  { id: '5', trainee_id: 'T004', scenario_id: 'gas_leak', score: 60, mistakes: 3, duration_seconds: 180, passed: false },
];

export default function App() {
  const totalTrainees = new Set(mockData.map(d => d.trainee_id)).size;
  const completedTraining = mockData.length;
  const passedCount = mockData.filter(d => d.passed).length;
  const failedCount = mockData.filter(d => !d.passed).length;

  return (
    <div className="min-h-screen bg-gray-50 text-gray-800 font-sans">
      <header className="bg-white shadow-sm border-b border-gray-200 px-8 py-4 flex items-center gap-3">
        <ShieldAlert className="text-blue-600 w-8 h-8" />
        <h1 className="text-xl font-bold text-gray-900">AR Safety Simulator Dashboard</h1>
      </header>

      <main className="max-w-7xl mx-auto p-8">
        
        {/* KPI Cards */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
          <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-6 flex items-center gap-4">
            <div className="p-3 bg-blue-50 text-blue-600 rounded-lg">
              <Users className="w-6 h-6" />
            </div>
            <div>
              <p className="text-sm font-medium text-gray-500">Total Trainees</p>
              <h3 className="text-2xl font-bold text-gray-900">{totalTrainees}</h3>
            </div>
          </div>
          
          <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-6 flex items-center gap-4">
            <div className="p-3 bg-indigo-50 text-indigo-600 rounded-lg">
              <Clock className="w-6 h-6" />
            </div>
            <div>
              <p className="text-sm font-medium text-gray-500">Completed Sessions</p>
              <h3 className="text-2xl font-bold text-gray-900">{completedTraining}</h3>
            </div>
          </div>

          <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-6 flex items-center gap-4">
            <div className="p-3 bg-green-50 text-green-600 rounded-lg">
              <CheckCircle className="w-6 h-6" />
            </div>
            <div>
              <p className="text-sm font-medium text-gray-500">Passed</p>
              <h3 className="text-2xl font-bold text-gray-900">{passedCount}</h3>
            </div>
          </div>

          <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-6 flex items-center gap-4">
            <div className="p-3 bg-red-50 text-red-600 rounded-lg">
              <XCircle className="w-6 h-6" />
            </div>
            <div>
              <p className="text-sm font-medium text-gray-500">Failed</p>
              <h3 className="text-2xl font-bold text-gray-900">{failedCount}</h3>
            </div>
          </div>
        </div>

        {/* Results Table */}
        <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
          <div className="px-6 py-4 border-b border-gray-100 bg-white">
            <h2 className="text-lg font-semibold text-gray-800">Recent Results</h2>
          </div>
          <div className="overflow-x-auto">
            <table className="w-full text-left border-collapse">
              <thead>
                <tr className="bg-gray-50 text-gray-500 text-sm border-b border-gray-100">
                  <th className="px-6 py-4 font-medium">Trainee ID</th>
                  <th className="px-6 py-4 font-medium">Scenario</th>
                  <th className="px-6 py-4 font-medium">Score</th>
                  <th className="px-6 py-4 font-medium">Mistakes</th>
                  <th className="px-6 py-4 font-medium">Duration</th>
                  <th className="px-6 py-4 font-medium">Status</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-100">
                {mockData.map((row) => (
                  <tr key={row.id} className="hover:bg-gray-50 transition-colors">
                    <td className="px-6 py-4 text-sm font-medium text-gray-900">{row.trainee_id}</td>
                    <td className="px-6 py-4 text-sm text-gray-600 capitalize">
                      {row.scenario_id.replace('_', ' ')}
                    </td>
                    <td className="px-6 py-4 text-sm font-semibold text-gray-700">{row.score}</td>
                    <td className="px-6 py-4 text-sm text-gray-600">{row.mistakes}</td>
                    <td className="px-6 py-4 text-sm text-gray-600">{row.duration_seconds}s</td>
                    <td className="px-6 py-4 text-sm">
                      <span className={`inline-flex items-center px-2.5 py-1 rounded-full text-xs font-semibold ${row.passed ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'}`}>
                        {row.passed ? 'Passed' : 'Failed'}
                      </span>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      </main>
    </div>
  );
}
