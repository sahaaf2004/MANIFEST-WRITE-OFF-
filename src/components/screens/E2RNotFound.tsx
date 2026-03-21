import React from 'react';
import { useAppContext } from '../../context/AppContext';
import { SearchX } from 'lucide-react';

export const E2RNotFound: React.FC = () => {
  const { state, updateState } = useAppContext();

  return (
    <div className="max-w-2xl mx-auto px-4">
      <div className="bg-white rounded-xl shadow-sm border border-[var(--danger)] overflow-hidden">
        <div className="bg-[var(--danger)] text-white p-4 font-bold text-lg text-center">
          R-Number Not Found
        </div>
        <div className="p-8 flex flex-col items-center text-center">
          <SearchX className="w-16 h-16 text-[var(--danger)] mb-6" />
          <h2 className="text-2xl font-bold text-gray-900 mb-4">Please verify the R-Number and ensure it matches the correct port and year.</h2>
          
          <div className="bg-gray-50 rounded-lg p-6 w-full max-w-md mb-8 border border-gray-200 text-left font-mono text-sm space-y-2">
            <div className="flex justify-between">
              <span className="text-gray-500">R-Number:</span>
              <span className="font-bold text-gray-900">{state.rNumber || 'R-2024-0001'}</span>
            </div>
            <div className="flex justify-between">
              <span className="text-gray-500">Port:</span>
              <span className="font-bold text-gray-900">{state.port || 'Malé Commercial Harbour'}</span>
            </div>
            <div className="flex justify-between pt-2 border-t border-gray-200 mt-2">
              <span className="text-gray-500">Result:</span>
              <span className="font-bold text-[var(--danger)]">Not found in ASYCUDA</span>
            </div>
          </div>

          <div className="flex flex-col gap-4 w-full max-w-sm">
            <button 
              onClick={() => updateState({ status: 'idle' })}
              className="btn-primary w-full"
            >
              Try a Different R-Number
            </button>
            <a href="#" className="text-[var(--primary)] hover:underline text-sm font-medium">
              Contact Customs Support
            </a>
          </div>
        </div>
      </div>
    </div>
  );
};
