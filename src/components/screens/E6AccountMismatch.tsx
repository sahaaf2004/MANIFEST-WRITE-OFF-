import React from 'react';
import { useAppContext } from '../../context/AppContext';
import { AlertTriangle } from 'lucide-react';

export const E6AccountMismatch: React.FC = () => {
  const { state, updateState } = useAppContext();

  return (
    <div className="max-w-2xl mx-auto px-4">
      <div className="bg-white rounded-xl shadow-sm border border-[var(--danger)] overflow-hidden">
        <div className="bg-[var(--danger)] text-white p-4 font-bold text-lg text-center">
          Account Not Compatible with Consignee
        </div>
        <div className="p-8 flex flex-col items-center text-center">
          <AlertTriangle className="w-16 h-16 text-[var(--danger)] mb-6" />
          <h2 className="text-2xl font-bold text-gray-900 mb-4">Payment account mismatch</h2>
          
          <div className="bg-gray-50 rounded-lg p-6 w-full max-w-md mb-6 border border-gray-200 text-left font-mono text-sm space-y-2">
            <div className="flex justify-between">
              <span className="text-gray-500">FF Account:</span>
              <span className="font-bold text-gray-900">{state.manifestData?.ffAccount || 'FF-12345'}</span>
            </div>
            <div className="flex justify-between">
              <span className="text-gray-500">Consignee Code:</span>
              <span className="font-bold text-gray-900">{state.manifestData?.consigneeCode || 'C-98765'}</span>
            </div>
            <div className="flex justify-between pt-2 border-t border-gray-200 mt-2">
              <span className="text-gray-500">Status:</span>
              <span className="font-bold text-[var(--danger)]">Incompatible</span>
            </div>
          </div>

          <p className="text-gray-600 mb-8 max-w-md">
            The registered payment account does not match the consignee code on this manifest. Please contact the Maldives Customs Service to resolve this.
          </p>

          <div className="bg-blue-50 text-blue-800 p-4 rounded-lg text-sm w-full max-w-md mb-8 text-left">
            <p className="font-bold mb-1">Customs Finance Department</p>
            <p>Phone: 333 4193</p>
            <p>Email: finance@customs.gov.mv</p>
          </div>

          <div className="flex flex-col gap-4 w-full max-w-sm">
            <button 
              onClick={() => updateState({ status: 'idle' })}
              className="btn-ghost w-full"
            >
              Back to Search
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};
