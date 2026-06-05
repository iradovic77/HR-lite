import { useEffect, useState } from 'react'
import { useTranslation } from 'react-i18next'
import {
  Table, Button, Tag, Space, Form, Input,
  InputNumber, Switch, App, Pagination, Tooltip, Checkbox,
} from 'antd'
import AppModal from '@/components/AppModal'
import { PlusOutlined, EditOutlined, StopOutlined, CheckOutlined, DeleteOutlined } from '@ant-design/icons'
import type { ColumnsType } from 'antd/es/table'
import { genderApi, type GenderResponse, type CreateGenderRequest } from '@/api/codebook'
import CodebookLayout from '@/layouts/CodebookLayout'

const PAGE_SIZE = 20
type FormValues = Omit<GenderResponse, 'id'>

export default function GenderPage() {
  const { t } = useTranslation()
  const { message, modal } = App.useApp()

  const [data, setData]               = useState<GenderResponse[]>([])
  const [loading, setLoading]         = useState(false)
  const [page, setPage]               = useState(1)
  const [onlyActive, setOnlyActive]   = useState(true)
  const [modalOpen, setModalOpen]     = useState(false)
  const [saving, setSaving]           = useState(false)
  const [editingItem, setEditingItem] = useState<GenderResponse | null>(null)
  const [form] = Form.useForm<FormValues>()

  // ── Dohvat podataka ─────────────────────────────────────────────────

  const fetchData = async () => {
    setLoading(true)
    try {
      const res = await genderApi.getAll(true)
      setData(res.data)
      setPage(1)
    } catch {
      message.error('Greška pri dohvatu podataka')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => { fetchData() }, [])

  const filteredData  = onlyActive ? data.filter(d => d.isActive) : data
  const paginatedData = filteredData.slice((page - 1) * PAGE_SIZE, page * PAGE_SIZE)

  // ── Handlers ────────────────────────────────────────────────────────

  const openAddModal = () => {
    setEditingItem(null)
    form.resetFields()
    form.setFieldsValue({ isActive: true, ordinal: data.length + 1 })
    setModalOpen(true)
  }

  const openEditModal = (item: GenderResponse) => {
    setEditingItem(item)
    form.setFieldsValue(item)
    setModalOpen(true)
  }

  const handleSave = async () => {
    const values = await form.validateFields()
    setSaving(true)
    try {
      if (editingItem) {
        await genderApi.update(editingItem.id, values)
        message.success('Zapis ažuriran')
      } else {
        await genderApi.create(values as CreateGenderRequest)
        message.success('Zapis dodan')
      }
      setModalOpen(false)
      fetchData()
    } catch {
      message.error('Greška pri spremanju')
    } finally {
      setSaving(false)
    }
  }

  const handleToggleActive = async (item: GenderResponse) => {
    try {
      await genderApi.toggleActive(item.id)
      message.success(item.isActive ? 'Zapis deaktiviran' : 'Zapis aktiviran')
      fetchData()
    } catch {
      message.error('Greška pri promjeni statusa')
    }
  }

  const handleDelete = async (item: GenderResponse) => {
    try {
      await genderApi.deleteById(item.id)
      message.success('Zapis obrisan')
      fetchData()
    } catch (err: unknown) {
      const status = (err as { response?: { status?: number } })?.response?.status
      message.error(status === 409
        ? t('common.error_in_use')
        : t('common.error_delete'))
    }
  }

  // ── Confirm dijalozi (centrirani na ekranu) ─────────────────────────

  const confirmToggleActive = (item: GenderResponse) => {
    modal.confirm({
      title: t(item.isActive
        ? 'codebook.gender.confirm.deactivate'
        : 'codebook.gender.confirm.activate'),
      onOk: () => handleToggleActive(item),
      okText: t('common.yes'),
      cancelText: t('common.no'),
      centered: true,
    })
  }

  const confirmDelete = (item: GenderResponse) => {
    modal.confirm({
      title: t('codebook.gender.confirm.delete'),
      content: t('codebook.gender.confirm.delete_description'),
      onOk: () => handleDelete(item),
      okText: t('codebook.gender.confirm.delete_ok'),
      okType: 'danger',
      cancelText: t('common.no'),
      centered: true,
    })
  }

  // ── Kolone tablice ──────────────────────────────────────────────────

  const columns: ColumnsType<GenderResponse> = [
    {
      title: t('codebook.gender.columns.code'),
      dataIndex: 'code',
      key: 'code',
      width: 80,
      sorter: (a, b) => a.code.localeCompare(b.code),
    },
    {
      title: t('codebook.gender.columns.nameHr'),
      dataIndex: 'nameHr',
      key: 'nameHr',
    },
    {
      title: t('codebook.gender.columns.nameEn'),
      dataIndex: 'nameEn',
      key: 'nameEn',
      render: (val: string | null) => val ?? '—',
    },
    {
      title: t('codebook.gender.columns.ordinal'),
      dataIndex: 'ordinal',
      key: 'ordinal',
      width: 110,
      sorter: (a, b) => a.ordinal - b.ordinal,
      defaultSortOrder: 'ascend',
    },
    {
      title: t('codebook.gender.columns.status'),
      dataIndex: 'isActive',
      key: 'isActive',
      width: 110,
      render: (isActive: boolean) =>
        isActive
          ? <Tag color="success">{t('codebook.gender.status.active')}</Tag>
          : <Tag color="default">{t('codebook.gender.status.inactive')}</Tag>,
    },
    {
      title: t('codebook.gender.columns.actions'),
      key: 'actions',
      width: 120,
      render: (_, record) => (
        <Space size={4}>
          <Tooltip title={t('codebook.gender.actions.edit')}>
            <Button
              type="text" icon={<EditOutlined />} size="small"
              onClick={() => openEditModal(record)}
            />
          </Tooltip>

          <Tooltip title={t(record.isActive
            ? 'codebook.gender.actions.deactivate'
            : 'codebook.gender.actions.activate')}
          >
            <Button
              type="text"
              icon={record.isActive ? <StopOutlined /> : <CheckOutlined />}
              size="small"
              danger={record.isActive}
              onClick={() => confirmToggleActive(record)}
            />
          </Tooltip>

          <Tooltip title={t('codebook.gender.actions.delete')}>
            <Button
              type="text" icon={<DeleteOutlined />} size="small" danger
              onClick={() => confirmDelete(record)}
            />
          </Tooltip>
        </Space>
      ),
    },
  ]

  // ── Render ──────────────────────────────────────────────────────────

  return (
    <CodebookLayout
      title={t('codebook.gender.title')}
      extra={
        <Space>
          <Checkbox
            checked={onlyActive}
            onChange={e => { setOnlyActive(e.target.checked); setPage(1) }}
          >
            {t('common.only_active')}
          </Checkbox>
          <Button type="primary" icon={<PlusOutlined />} onClick={openAddModal}>
            {t('codebook.gender.addNew')}
          </Button>
        </Space>
      }
      pagination={
        <Pagination
          current={page}
          total={filteredData.length}
          pageSize={PAGE_SIZE}
          onChange={setPage}
          showTotal={(total) => `Ukupno: ${total}`}
          size="small"
        />
      }
    >
      <Table
        columns={columns}
        dataSource={paginatedData}
        rowKey="id"
        size="small"
        loading={loading}
        pagination={false}
        onRow={(record) => ({
          style: record.isActive ? {} : { opacity: 0.45 },
        })}
      />

      <AppModal
        title={editingItem
          ? t('codebook.gender.modal.editTitle')
          : t('codebook.gender.modal.addTitle')}
        open={modalOpen}
        onOk={handleSave}
        onCancel={() => setModalOpen(false)}
        okText={t('common.save')}
        cancelText={t('common.cancel')}
        confirmLoading={saving}
        destroyOnClose
      >
        <Form form={form} layout="vertical" style={{ marginTop: 16 }}>
          <Form.Item
            name="code"
            label={t('codebook.gender.modal.code')}
            tooltip={t('codebook.gender.modal.code_hint')}
            rules={[{ required: true, message: t('common.required') }]}
          >
            <Input maxLength={10} style={{ textTransform: 'uppercase' }} />
          </Form.Item>

          <Form.Item
            name="nameHr"
            label={t('codebook.gender.modal.nameHr')}
            rules={[{ required: true, message: t('common.required') }]}
          >
            <Input maxLength={100} />
          </Form.Item>

          <Form.Item name="nameEn" label={t('codebook.gender.modal.nameEn')}>
            <Input maxLength={100} />
          </Form.Item>

          <Form.Item name="ordinal" label={t('codebook.gender.modal.ordinal')}>
            <InputNumber min={0} style={{ width: '100%' }} />
          </Form.Item>

          <Form.Item name="isActive" label={t('codebook.gender.modal.isActive')} valuePropName="checked">
            <Switch />
          </Form.Item>
        </Form>
      </AppModal>
    </CodebookLayout>
  )
}
