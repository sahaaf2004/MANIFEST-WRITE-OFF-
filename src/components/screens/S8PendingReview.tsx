import React, { useEffect, useState } from 'react';
import { useAppContext } from '../../context/AppContext';
import { Clock, CheckCircle, ChevronDown, ChevronUp } from 'lucide-react';

export const S8PendingReview: React.FC = () => {
  const { state, updateState } = useAppContext();
  const [showSummary, setShowSummary] = useState(false);
  const [isApproved, setIsApproved] = useState(false);

  // Poll for status
  useEffect(() => {
    const timer = setInterval(() => {
      // Mocking status change after 5 seconds for demo
      setIsApproved(true);
    }, 5000);

    return () => clearInterval(timer);
  }, []);

  return (
    <div className="max-w-4xl mx-auto px-4">
      <div className="bg-white rounded-xl shadow-sm border border-gray-200 card-primary-stripe overflow-hidden">
        <div className="p-6 sm:p-8 border-b border-gray-100">
          <h2 className="text-2xl font-bold text-gray-900 mb-2">Amendment Application Status</h2>
          <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
            <div>
              <p className="text-sm text-gray-500 mb-1">Reference Number</p>
              <p className="font-mono text-2xl font-bold text-[var(--primary)]">
                {state.amendmentRef || `AMD-${new Date().getFullYear()}-102938`}
              </p>
            </div>
            <div className="text-left sm:text-right">
              <p className="text-sm text-gray-500 mb-1">Submitted</p>
              <p className="font-medium text-gray-900">
                {new Date().toLocaleString('en-GB', { day: '2-digit', month: 'short', year: 'numeric', hour: '2-digit', minute: '2-digit' })}
              </p>
            </div>
          </div>
        </div>

        <div className="p-6 sm:p-8">
          {/* Tracker */}
          <div className="mb-12 relative">
            <div className="absolute top-1/2 left-0 w-full h-1 bg-gray-200 -translate-y-1/2 z-0 rounded-full"></div>
            <div 
              className="absolute top-1/2 left-0 h-1 bg-[var(--success)] -translate-y-1/2 z-0 rounded-full transition-all duration-500"
              style={{ width: isApproved ? '66%' : '33%' }}
            ></div>
            
            <div className="relative z-10 flex justify-between">
              <div className="flex flex-col items-center gap-2">
                <div className="w-8 h-8 rounded-full bg-[var(--success)] text-white flex items-center justify-center shadow-sm">
                  <CheckCircle size={16} />
                </div>
                <span className="text-xs font-medium text-gray-900">Submitted</span>
              </div>
              
              <div className="flex flex-col items-center gap-2">
                <div className={`w-8 h-8 rounded-full flex items-center justify-center shadow-sm transition-colors ${isApproved ? 'bg-[var(--success)] text-white' : 'bg-[var(--primary)] text-white animate-pulse'}`}>
                  {isApproved ? <CheckCircle size={16} /> : <span className="w-2.5 h-2.5 bg-white rounded-full"></span>}
                </div>
                <span className={`text-xs font-medium ${isApproved ? 'text-gray-900' : 'text-[var(--primary)]'}`}>Pending Review</span>
              </div>
              
              <div className="flex flex-col items-center gap-2">
                <div className={`w-8 h-8 rounded-full flex items-center justify-center shadow-sm transition-colors ${isApproved ? 'bg-[var(--success)] text-white' : 'bg-gray-200 text-gray-400'}`}>
                  {isApproved ? <CheckCircle size={16} /> : <span className="w-2.5 h-2.5 bg-gray-400 rounded-full"></span>}
                </div>
                <span className={`text-xs font-medium ${isApproved ? 'text-gray-900' : 'text-gray-400'}`}>Admin Decision</span>
              </div>
              
              <div className="flex flex-col items-center gap-2">
                <div className="w-8 h-8 rounded-full bg-gray-200 text-gray-400 flex items-center justify-center shadow-sm">
                  <span className="w-2.5 h-2.5 bg-gray-400 rounded-full"></span>
                </div>
                <span className="text-xs font-medium text-gray-400">Payment & Finalization</span>
              </div>
            </div>
          </div>

          {/* Status Message */}
          {isApproved ? (
            <div className="bg-[var(--success-bg)] border border-green-200 rounded-xl p-6 mb-8 flex items-start gap-4">
              <CheckCircle className="w-6 h-6 text-[var(--success)] shrink-0 mt-0.5" />
              <div>
                <h3 className="text-lg font-bold text-green-900 mb-1">Application Approved</h3>
                <p className="text-green-800 mb-4">Your amendment application has been reviewed and approved by a Customs Officer. Please proceed to payment to finalize the write-off.</p>
                <button 
                  onClick={() => updateState({ status: 'payment_ready' })}
                  className="btn-primary"
                >
                  Proceed to Payment
                </button>
              </div>
            </div>
          ) : (
            <div className="bg-[var(--warning-bg)] border border-blue-200 rounded-xl p-6 mb-8 flex items-start gap-4">
              <Clock className="w-6 h-6 text-[var(--warning)] shrink-0 mt-0.5 animate-pulse" />
              <div>
                <h3 className="text-lg font-bold text-blue-900 mb-1">Awaiting Review</h3>
                <p className="text-blue-800">Your application is in the queue and awaiting review by a Customs Officer. You will be notified by email when a decision is made.</p>
              </div>
            </div>
          )}

          {/* Summary Accordion */}
          <div className="border border-gray-200 rounded-lg overflow-hidden mb-8">
            <button 
              onClick={() => setShowSummary(!showSummary)}
              className="w-full flex items-center justify-between p-4 bg-gray-50 hover:bg-gray-100 transition-colors"
            >
              <span className="font-medium text-gray-900">View Application Summary</span>
              {showSummary ? <ChevronUp size={20} className="text-gray-500" /> : <ChevronDown size={20} className="text-gray-500" />}
            </button>
            
            {showSummary && (
              <div className="p-6 bg-white border-t border-gray-200">
                <div className="grid grid-cols-1 sm:grid-cols-2 gap-6">
                  <div>
                    <p className="text-sm text-gray-500 mb-1">R-Number</p>
                    <p className="font-medium text-gray-900">{state.rNumber || 'R-2024-0001'}</p>
                  </div>
                  <div>
                    <p className="text-sm text-gray-500 mb-1">Port</p>
                    <p className="font-medium text-gray-900">{state.port || 'Malé Commercial Harbour'}</p>
                  </div>
                  <div className="sm:col-span-2">
                    <p className="text-sm text-gray-500 mb-1">Amended Fields</p>
                    <div className="flex flex-wrap gap-2 mt-2">
                      {state.failedFields?.map(f => (
                        <span key={f.field} className="px-2 py-1 bg-gray-100 text-gray-700 rounded text-sm border border-gray-200">
                          {f.field}
                        </span>
                      )) || <span className="text-gray-500 italic">No fields selected</span>}
                    </div>
                  </div>
                  <div className="sm:col-span-2">
                    <p className="text-sm text-gray-500 mb-1">Calculated Fine</p>
                    <p className="font-bold text-[var(--danger)]">MVR {state.calculatedFine?.toLocaleString('en-US', {minimumFractionDigits: 2, maximumFractionDigits: 2}) || '1,000.00'}</p>
                  </div>
                </div>
              </div>
            )}
          </div>

          <div className="flex flex-col sm:flex-row gap-4">
            {!isApproved && (
              <button 
                onClick={() => setIsApproved(true)} // Mocking manual check
                className="btn-ghost flex-1"
              >
                Check Status
              </button>
            )}
            <button 
              onClick={() => updateState({ status: 'idle' })}
              className="btn-ghost flex-1"
            >
              Back to Dashboard
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};
