import { useEffect, useMemo, useState } from 'react'
import { useTranslation } from 'react-i18next'
import {
  Button, Tag, Space, Form, Input,
  InputNumber, Switch, App, Tooltip, Checkbox,
} from 'antd'
import AppModal from '@/components/AppModal'
import AgGridWrapper from '@/components/AgGridWrapper'
import { PlusOutlined, EditOutlined, StopOutlined, CheckOutlined, DeleteOutlined } from '@ant-design/icons'
import type { ColDef, ICellRendererParams } from 'ag-grid-community'
import { genderApi, type GenderResponse, type CreateGenderRequest } from '@/api/codebook'
import CodebookLayout from '@/layouts/CodebookLayout'

type FormValues = Omit<GenderResponse, 'id'>

export default function GenderPage() {
  const { t } = useTranslation()
  const { message, modal } = App.useApp()

  const [data, setData]               = useState<GenderResponse[]>([])
  const [loading, setLoading]         = useState(false)
  const [onlyActive, setOnlyActive]   = useState(true)
  const [modalOpen, setModalOpen]     = useState(false)
  const [saving, setSaving]           = useState(false)
  const [editingItem, setEditingItem] = useState<GenderResponse | null>(null)
  const [form] = Form.useForm<FormValues>()

  const fetchData = async () => {
    setLoading(true)
    try {
      const res = await genderApi.getAll(true)
      setData(res.data)
    } catch {
      message.error('Greška pri dohvatu podataka')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => { fetchData() }, [])

  const filteredData = onlyActive ? data.filter(d => d.isActive) : data

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
      message.error(status === 409 ? t('common.error_in_use') : t('common.error_delete'))
    }
  }

  const confirmToggleActive = (item: GenderResponse) => {
    modal.confirm({
      title: t(item.isActive ? 'codebook.gender.confirm.deactivate' : 'codebook.gender.confirm.activate'),
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

  const columnDefs = useMemo<ColDef<GenderResponse>[]>(() => [
    {
      field: 'code',
      headerName: t('codebook.gender.columns.code'),
      width: 80,
    },
    {
      field: 'nameHr',
      headerName: t('codebook.gender.columns.nameHr'),
      flex: 1,
    },
    {
      field: 'nameEn',
      headerName: t('codebook.gender.columns.nameEn'),
      flex: 1,
      valueFormatter: (p) => p.value ?? '—',
    },
    {
      field: 'ordinal',
      headerName: t('codebook.gender.columns.ordinal'),
      width: 120,
      sort: 'asc',
    },
    {
      field: 'isActive',
      headerName: t('codebook.gender.columns.status'),
      width: 120,
      filter: false,
      cellRenderer: (p: ICellRendererParams<GenderResponse>) =>
        p.value
          ? <Tag color="success">{t('codebook.gender.status.active')}</Tag>
          : <Tag color="default">{t('codebook.gender.status.inactive')}</Tag>,
    },
    {
      headerName: t('codebook.gender.columns.actions'),
      width: 120,
      sortable: false,
      filter: false,
      resizable: false,
      cellRenderer: (p: ICellRendererParams<GenderResponse>) => {
        const rec = p.data!
        return (
          <Space size={4}>
            <Tooltip title={t('codebook.gender.actions.edit')}>
              <Button type="text" icon={<EditOutlined />} size="small" onClick={() => openEditModal(rec)} />
            </Tooltip>
            <Tooltip title={t(rec.isActive ? 'codebook.gender.actions.deactivate' : 'codebook.gender.actions.activate')}>
              <Button type="text" icon={rec.isActive ? <StopOutlined /> : <CheckOutlined />}
                size="small" danger={rec.isActive} onClick={() => confirmToggleActive(rec)} />
            </Tooltip>
            <Tooltip title={t('codebook.gender.actions.delete')}>
              <Button type="text" icon={<DeleteOutlined />} size="small" danger onClick={() => confirmDelete(rec)} />
            </Tooltip>
          </Space>
        )
      },
    },
  ], [t, openEditModal, confirmToggleActive, confirmDelete])

  return (
    <CodebookLayout
      title={t('codebook.gender.title')}
      extra={
        <Space>
          <Checkbox checked={onlyActive} onChange={e => setOnlyActive(e.target.checked)}>
            {t('common.only_active')}
          </Checkbox>
          <Button type="primary" icon={<PlusOutlined />} onClick={openAddModal}>
            {t('codebook.gender.addNew')}
          </Button>
        </Space>
      }
    >
      <AgGridWrapper<GenderResponse>
        columnDefs={columnDefs}
        rowData={filteredData}
        loading={loading}
        getRowId={(p) => p.data.id}
        getRowStyle={(p) => p.data?.isActive ? undefined : { opacity: 0.45 }}
      />

      <AppModal
        title={editingItem ? t('codebook.gender.modal.editTitle') : t('codebook.gender.modal.addTitle')}
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
