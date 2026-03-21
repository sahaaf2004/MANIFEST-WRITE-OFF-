import React from 'react';
import { useAppContext } from '../../context/AppContext';
import { AlertTriangle } from 'lucide-react';

export const E1AyscudaDown: React.FC = () => {
  const { updateState } = useAppContext();

  return (
    <div className="max-w-2xl mx-auto px-4">
      <div className="bg-white rounded-xl shadow-sm border border-[var(--danger)] overflow-hidden">
        <div className="bg-[var(--danger)] text-white p-4 font-bold text-lg text-center">
          ASYCUDA System Unavailable
        </div>
        <div className="p-8 flex flex-col items-center text-center">
          <AlertTriangle className="w-16 h-16 text-[var(--danger)] mb-6" />
          <h2 className="text-2xl font-bold text-gray-900 mb-4">Cannot connect to the ASYCUDA system</h2>
          <p className="text-gray-600 mb-8 max-w-md">
            The customs system is currently unavailable. This is a temporary issue. Please try again in a few minutes.
          </p>
          
          <div className="bg-gray-50 rounded-lg p-4 w-full max-w-sm mb-8 border border-gray-200">
            <p className="text-sm font-mono text-gray-500">
              Last checked: {new Date().toLocaleString('en-GB', { day: '2-digit', month: 'short', year: 'numeric', hour: '2-digit', minute: '2-digit' })}
            </p>
          </div>

          <div className="flex flex-col sm:flex-row gap-4 w-full max-w-sm">
            <button 
              onClick={() => updateState({ status: 'loading' })}
              className="btn-primary flex-1"
            >
              Try Again
            </button>
            <button 
              onClick={() => updateState({ status: 'idle' })}
              className="btn-ghost flex-1"
            >
              Back to Search
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};
