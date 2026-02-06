import Modal from '@/components/common/Modal';
import { DetailField, DetailSection, StatusBadge } from '@/pages/Requests/DetailField';
import { User, Calendar, FileText, Clock, CheckCircle } from 'lucide-react';
import type { LeaveRequestResponse } from '@/pages/Requests/LeaveRequestsType';

type LeaveRequestDetailModalProps = {
    isOpen: boolean;
    onClose: () => void;
    request: LeaveRequestResponse | null;
};

export default function LeaveRequestDetailModal({ isOpen, onClose, request }: LeaveRequestDetailModalProps) {
    if (!request) return null;

    const formatDate = (dateString: string) => {
        const options: Intl.DateTimeFormatOptions = { year: 'numeric', month: 'short', day: 'numeric' };
        return new Date(dateString).toLocaleDateString(undefined, options);
    };

    const formatDateTime = (dateString: string) => {
        const options: Intl.DateTimeFormatOptions = {
        year: 'numeric', month: 'short', day: 'numeric',
        hour: '2-digit', minute: '2-digit', second: '2-digit'
        };
        return new Date(dateString).toLocaleString(undefined, options);
    };

    return (
        <Modal isOpen={isOpen} onClose={onClose} title="Leave Request Details" size="lg">
            <div className="space-y-2">
                {/* Employee Info */}
                <DetailSection title="Employee Information">
                    <DetailField 
                        icon={<User size={18} />}
                        label="Employee Name"
                        value={request.userName}
                    />
                </DetailSection>

                {/* Leave Details */}
                <DetailSection title="Leave Details">
                    <DetailField 
                        icon={<FileText size={18} />}
                        label="Leave Type"
                        value={
                            <span className="inline-flex items-center px-3 py-1 rounded-lg bg-blue-50 text-blue-700 font-medium text-sm">
                                {request.type}
                            </span>
                        }
                    />
                    <DetailField 
                        icon={<Calendar size={18} />}
                        label="Duration"
                        value={
                            <div className="flex items-center gap-2">
                                <span className="px-2 py-1 bg-gray-100 rounded-lg text-sm">{formatDate(request.startDate)}</span>
                                <span className="text-gray-400">→</span>
                                <span className="px-2 py-1 bg-gray-100 rounded-lg text-sm">{formatDate(request.endDate)}</span>
                            </div>
                        }
                    />
                    <DetailField 
                        label="Reason"
                        value={
                            <p className="text-gray-600 bg-white p-3 rounded-lg border border-gray-100">
                                {request.reason}
                            </p>
                        }
                    />
                </DetailSection>

                {/* Status & Approval */}
                <DetailSection title="Status & Approval">
                    <DetailField 
                        icon={<Clock size={18} />}
                        label="Status"
                        value={<StatusBadge status={request.status} />}
                    />
                    {request.createdAt && (
                        <DetailField 
                            label="Submitted At"
                            value={formatDateTime(request.createdAt)}
                        />
                    )}
                    {/* {request.approvedBy && (
                        <DetailField 
                            icon={<CheckCircle size={18} />}
                            label="Approved By"
                            value={request.approvedBy}
                        />
                    )}
                    {request.approvedAt && (
                        <DetailField 
                            label="Approved At"
                            value={request.approvedAt}
                        />
                    )} */}
                </DetailSection>
            </div>
        </Modal>
    );
}
