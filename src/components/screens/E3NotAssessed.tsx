import React from 'react';
import { useAppContext } from '../../context/AppContext';
import { Clock } from 'lucide-react';

export const E3NotAssessed: React.FC = () => {
  const { updateState } = useAppContext();

  return (
    <div className="max-w-2xl mx-auto px-4">
      <div className="bg-white rounded-xl shadow-sm border border-[var(--warning)] overflow-hidden">
        <div className="bg-[var(--warning)] text-white p-4 font-bold text-lg text-center">
          Form Pending Assessment
        </div>
        <div className="p-8 flex flex-col items-center text-center">
          <Clock className="w-16 h-16 text-[var(--warning)] mb-6 animate-pulse" />
          <h2 className="text-2xl font-bold text-gray-900 mb-4">Your manifest has not yet been assessed</h2>
          <p className="text-gray-600 mb-8 max-w-md">
            A customs officer has not yet processed this form. Once assessed, you will be able to proceed with the write-off.
          </p>
          
          <div className="bg-[var(--warning-bg)] rounded-lg p-4 w-full max-w-sm mb-8 border border-blue-200">
            <p className="text-sm font-medium text-blue-800">
              Forms are typically assessed within 1–2 working days
            </p>
          </div>

          <div className="flex flex-col sm:flex-row gap-4 w-full max-w-sm">
            <button 
              onClick={() => updateState({ status: 'loading' })}
              className="btn-primary flex-1"
            >
              Check Again
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
