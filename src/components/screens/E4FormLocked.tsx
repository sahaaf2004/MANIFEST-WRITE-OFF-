import React from 'react';
import { useAppContext } from '../../context/AppContext';
import { Lock } from 'lucide-react';

export const E4FormLocked: React.FC = () => {
  const { updateState } = useAppContext();

  return (
    <div className="max-w-2xl mx-auto px-4">
      <div className="bg-white rounded-xl shadow-sm border border-[var(--warning)] overflow-hidden">
        <div className="bg-[var(--warning)] text-white p-4 font-bold text-lg text-center">
          Form Currently Locked
        </div>
        <div className="p-8 flex flex-col items-center text-center">
          <Lock className="w-16 h-16 text-[var(--warning)] mb-6" />
          <h2 className="text-2xl font-bold text-gray-900 mb-4">This form is currently open by a customs officer</h2>
          <p className="text-gray-600 mb-8 max-w-md">
            The ASYCUDA system prevents access while an officer has this form open. Please wait a few minutes and try again.
          </p>
          
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
